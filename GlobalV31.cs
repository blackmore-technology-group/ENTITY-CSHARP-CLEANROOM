using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

public static class GlobalV31 {
 static readonly JavaScriptSerializer JS=new JavaScriptSerializer();
 static readonly HashSet<string> Kinds=new HashSet<string>{"SCHEMA","RIGHT","EVENT","CAPABILITY","ASSET_CLASS","TRUST_FRAMEWORK","DISPUTE_AUTHORITY","ATTESTATION_CLASS"};
 static readonly HashSet<string> Topology=new HashSet<string>{"CORE","REGIONAL","EDGE","SATELLITE","OFFLINE"};
 static readonly HashSet<string> Scarcity=new HashSet<string>{"RIGHT","ENTITLEMENT","CAPACITY","DURATION","JURISDICTION","USAGE_QUANTITY","DERIVATION","PARTICIPATION","TRANSFERABILITY"};
 static Dictionary<string,object> O(object x){return x as Dictionary<string,object>??new Dictionary<string,object>();}
 static object[] A(object x){return x as object[]??new object[0];}
 static string S(object x,string k){object v;return O(x).TryGetValue(k,out v)&&v!=null?Convert.ToString(v,CultureInfo.InvariantCulture):"";}
 static long I(object x,string k){object v;return O(x).TryGetValue(k,out v)&&v!=null?Convert.ToInt64(v,CultureInfo.InvariantCulture):0;}
 static bool B(object x,string k){object v;return O(x).TryGetValue(k,out v)&&v is bool&&(bool)v;}

 static string Canon(object v){
  if(v==null)return "null";var d=v as Dictionary<string,object>;
  if(d!=null){var ks=d.Keys.OrderBy(x=>x,StringComparer.Ordinal);return "{"+string.Join(",",ks.Select(k=>JS.Serialize(k)+":"+Canon(d[k])).ToArray())+"}";}
  var a=v as object[];if(a!=null)return "["+string.Join(",",a.Select(Canon).ToArray())+"]";
  if(v is ArrayList){var z=((ArrayList)v).Cast<object>().ToArray();return "["+string.Join(",",z.Select(Canon).ToArray())+"]";}
  if(v is string)return JS.Serialize((string)v);if(v is bool)return (bool)v?"true":"false";
  if(v is byte||v is sbyte||v is short||v is ushort||v is int||v is uint||v is long||v is ulong||v is decimal||v is double||v is float)return Convert.ToString(v,CultureInfo.InvariantCulture);
  return JS.Serialize(v);
 }
 static string Sha(byte[] b){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
 static string Sha(string s){return Sha(Encoding.UTF8.GetBytes(s));}
 static object Load(string p){return JS.DeserializeObject(File.ReadAllText(p,Encoding.UTF8).TrimStart('\uFEFF'));}
 static bool Hex64(object v,string k){var x=S(v,k);return x.Length==64&&x.All(c=>(c>='0'&&c<='9')||(c>='a'&&c<='f'));}
 static bool SortedUnique(object[] a){var s=a.Select(x=>Convert.ToString(x,CultureInfo.InvariantCulture)).ToArray();var z=s.Distinct().OrderBy(x=>x,StringComparer.Ordinal).ToArray();return s.SequenceEqual(z);}

 static bool ValidRecord(object rr){
  var r=O(rr);switch(S(r,"schema")){
   case "entity-v3-jurisdiction-profile-v1":
    if(!B(r,"legal_effect_is_deployment_specific"))return false;foreach(var x in A(r["rules"])){var e=S(x,"effect");if(e!="ALLOW"&&e!="REQUIRE"&&e!="PROHIBIT")return false;if(A(O(x)["actions"]).Length==0)return false;}return true;
   case "entity-v3-semantic-term-v1": return Kinds.Contains(S(r,"kind"))&&Hex64(r,"definition_sha256")&&S(r,"status")=="ACTIVE";
   case "entity-v3-topology-node-v1": return Topology.Contains(S(r,"topology_class"))&&B(r,"infrastructure_membership_is_not_sovereign_authority");
   case "entity-v3-purpose-bound-access-v1": return A(r["purposes"]).Length>0&&A(r["actions"]).Length>0&&I(r,"max_uses")>=0;
   case "entity-v3-offline-envelope-v1": return Hex64(r,"payload_sha256")&&I(r,"sequence")>=0&&I(r,"expires_at_ms")>I(r,"created_at_ms");
   case "entity-v3-crypto-transition-v1": return B(r,"downgrade_after_transition_prohibited")&&I(r,"old_retire_at_ms")>=I(r,"dual_sign_from_ms");
   case "entity-v3-data-economic-capital-v1": return B(r,"information_bytes_are_not_declared_scarce")&&Hex64(r,"provenance_root")&&Hex64(r,"content_sha256");
   case "entity-v3-bounded-economic-interest-v1":
    var a=A(r["actions"]);var sc=A(r["scarcity_sources"]);if(a.Length==0||!SortedUnique(a)||!B(r,"underlying_information_remains_nonrival"))return false;var has=false;foreach(var x in sc){var q=Convert.ToString(x,CultureInfo.InvariantCulture);if(!Scarcity.Contains(q))return false;if(q=="RIGHT")has=true;}var p=I(r,"participation_bps");return has&&p>=0&&p<=10000;
  }return false;
 }

 static bool VerifyChecksums(string root){
  foreach(var raw in File.ReadAllLines(Path.Combine(root,"SHA256SUMS.txt"))){var line=raw.TrimStart('\uFEFF');if(string.IsNullOrWhiteSpace(line))continue;var parts=line.Split(new[]{"  "},2,StringSplitOptions.None);if(parts.Length!=2)return false;var p=Path.Combine(root,parts[1].Replace('/',Path.DirectorySeparatorChar));if(!File.Exists(p)||Sha(File.ReadAllBytes(p))!=parts[0])return false;}return true;
 }

 public static Dictionary<string,object> Run(string repoRoot){
  var root=Path.Combine(repoRoot,"global-conformance-kit");var checksums=VerifyChecksums(root);
  var profile=O(Load(Path.Combine(root,"ENTITY_GLOBAL_CLEANROOM_PROFILE.json")));var manifest=O(Load(Path.Combine(root,"vectors","VECTOR_MANIFEST.json")));
  var rows=new List<Dictionary<string,object>>();foreach(var ee in A(manifest["vectors"])){var e=O(ee);var file=S(e,"file");var payload=O(Load(Path.Combine(root,"vectors",file)));var accepted=ValidRecord(payload["record"]);var expected=S(payload,"expect");rows.Add(new Dictionary<string,object>{{"name",Path.GetFileNameWithoutExtension(file)},{"accepted",accepted},{"expected",expected},{"ok",accepted==(expected=="VALID")}});}
  rows=rows.OrderBy(x=>S(x,"name"),StringComparer.Ordinal).ToList();var summary=new Dictionary<string,object>{{"schema","entity-v3.1-global-cleanroom-result-v1"},{"profile","ENTITY-GLOBAL-INFRASTRUCTURE"},{"doctrine_invariants",profile["doctrine_invariants"]},{"vectors",rows.ToArray()}};
  var result=Sha(Canon(summary));var passed=rows.Count(x=>B(x,"ok"));var valid=rows.Count(x=>B(x,"accepted"));var all=checksums&&passed==rows.Count&&result==S(profile,"expected_result_sha256")&&valid==I(profile,"valid_vectors")&&(rows.Count-valid)==I(profile,"invalid_vectors");
  return new Dictionary<string,object>{{"implementation","csharp"},{"checksums_pass",checksums},{"vectors_passed",passed},{"vectors_total",rows.Count},{"result_sha256",result},{"expected_result_sha256",S(profile,"expected_result_sha256")},{"doctrine_invariants",profile["doctrine_invariants"]},{"overall_valid",all},{"results",rows.ToArray()}};
 }
}
