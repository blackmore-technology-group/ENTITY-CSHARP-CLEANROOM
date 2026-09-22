using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;

class Program {
 static readonly JavaScriptSerializer JS=new JavaScriptSerializer();
 static readonly string[] Required={"transaction_record","manifests","asset_provenance","rights","licence","usage_receipt","license_settlement","value_record","digital_commodity","corporate_authorization","capital","share_settlement","event_ledger","external_trust_anchors"};
 static Dictionary<string,object> O(object x){return x as Dictionary<string,object> ?? new Dictionary<string,object>();}
 static object[] A(object x){return x as object[] ?? new object[0];}
 static string S(object x,string k){object v;return O(x).TryGetValue(k,out v)&&v!=null?Convert.ToString(v,CultureInfo.InvariantCulture):"";}
 static long I(object x,string k){object v;return O(x).TryGetValue(k,out v)&&v!=null?Convert.ToInt64(v,CultureInfo.InvariantCulture):0;}
 static bool B(object x,string k){object v;return O(x).TryGetValue(k,out v)&&v is bool&&(bool)v;}
 static void Add(List<string> e,string c){if(!e.Contains(c))e.Add(c);}
 static string Canon(object v){
  if(v==null)return "null";
  var d=v as Dictionary<string,object>;if(d!=null){var ks=d.Keys.OrderBy(x=>x,StringComparer.Ordinal);return "{"+string.Join(",",ks.Select(k=>JS.Serialize(k)+":"+Canon(d[k])).ToArray())+"}";}
  var a=v as object[];if(a!=null)return "["+string.Join(",",a.Select(Canon).ToArray())+"]";
  if(v is ArrayList){var z=((ArrayList)v).Cast<object>().ToArray();return "["+string.Join(",",z.Select(Canon).ToArray())+"]";}
  if(v is string)return JS.Serialize((string)v);if(v is bool)return (bool)v?"true":"false";
  if(v is byte||v is sbyte||v is short||v is ushort||v is int||v is uint||v is long||v is ulong||v is decimal||v is double||v is float)return Convert.ToString(v,CultureInfo.InvariantCulture);
  return JS.Serialize(v);
 }
 static string Sha(byte[] b){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
 static string Sha(string s){return Sha(Encoding.UTF8.GetBytes(s));}
 static object Load(string p){var t=File.ReadAllText(p,Encoding.UTF8).TrimStart('\uFEFF');return JS.DeserializeObject(t);}
 static bool VerifySig(Dictionary<string,object> bundle){
  try{var s=O(bundle["signature"]);if(S(s,"alg")!="Ed25519")return false;var key=(Ed25519PublicKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(S(s,"public_key_spki_der_b64")));var signer=new Ed25519Signer();signer.Init(false,key);var h=new Dictionary<string,object>{{"schema",S(bundle,"schema")},{"transaction_id",S(bundle,"transaction_id")},{"issuer_entity_id",S(bundle,"issuer_entity_id")},{"transaction_root_sha256",S(bundle,"transaction_root_sha256")}};var m=Encoding.UTF8.GetBytes(Canon(h));signer.BlockUpdate(m,0,m.Length);return signer.VerifySignature(Convert.FromBase64String(S(s,"sig_b64")));}catch{return false;}
 }
 static bool VerifyLedger(Dictionary<string,object> l,List<string> e){
  object evObj;if(!l.TryGetValue("events",out evObj)){Add(e,"LEDGER_SEQUENCE_INVALID");return false;}var events=A(evObj);var prev=new string('0',64);
  for(int n=0;n<events.Length;n++){var x=O(events[n]);if(I(x,"sequence")!=n+1){Add(e,"LEDGER_SEQUENCE_INVALID");return false;}if(S(x,"prev_hash")!=prev){Add(e,"LEDGER_PREV_HASH_INVALID");return false;}var expected=Sha(prev+":"+Canon(x["payload"]));if(S(x,"event_hash")!=expected){Add(e,"LEDGER_EVENT_HASH_INVALID");return false;}prev=expected;}
  var cp=O(l["checkpoint"]);if(I(cp,"sequence")!=events.Length||S(cp,"head_hash")!=prev){Add(e,"LEDGER_CHECKPOINT_INVALID");return false;}return true;
 }
 static Dictionary<string,object> Verify(Dictionary<string,object> bundle){
  var e=new List<string>();var ev=O(bundle["evidence"]);var root=Sha(Canon(ev));var rootOk=root==S(bundle,"transaction_root_sha256");if(!rootOk)Add(e,"ROOT_MISMATCH");var sigOk=VerifySig(bundle);if(!sigOk)Add(e,"SIGNATURE_INVALID");var reqOk=true;
  foreach(var section in Required)if(!ev.ContainsKey(section)){reqOk=false;Add(e,"MISSING_SECTION:"+section);}var cross=true;
  if(reqOk){var tr=O(ev["transaction_record"]);var p=O(ev["asset_provenance"]);var r=O(ev["rights"]);var l=O(ev["licence"]);var u=O(ev["usage_receipt"]);var st=O(ev["license_settlement"]);var vr=O(ev["value_record"]);var d=O(ev["digital_commodity"]);var ca=O(ev["corporate_authorization"]);var c=O(ev["capital"]);var ss=O(ev["share_settlement"]);
   if(S(p,"asset_id")!=S(tr,"asset_id")){cross=false;Add(e,"PROVENANCE_ASSET_MISMATCH");}if(S(r,"asset_id")!=S(tr,"asset_id")){cross=false;Add(e,"RIGHTS_ASSET_MISMATCH");}if(S(r,"claimant_entity_id")!=S(l,"grantor_entity_id")){cross=false;Add(e,"RIGHTS_CLAIMANT_MISMATCH");}
   if(S(l,"asset_id")!=S(tr,"asset_id")||S(l,"grantor_entity_id")!=S(tr,"grantor_entity_id")||S(l,"licensee_entity_id")!=S(tr,"licensee_entity_id")||S(l,"rights_claim_id")!=S(r,"claim_id")){cross=false;Add(e,"LICENCE_LINK_MISMATCH");}
   if(S(u,"licence_id")!=S(l,"licence_id")||S(u,"asset_id")!=S(tr,"asset_id")||S(u,"user_entity_id")!=S(l,"licensee_entity_id")){cross=false;Add(e,"USAGE_LINK_MISMATCH");}if(S(u,"purpose")!=S(l,"authorized_purpose")){cross=false;Add(e,"USAGE_PURPOSE_UNAUTHORIZED");}
   if(S(st,"licence_id")!=S(l,"licence_id")){cross=false;Add(e,"SETTLEMENT_LINK_MISMATCH");}if(S(st,"payer_entity_id")!=S(l,"licensee_entity_id")||S(st,"payee_entity_id")!=S(l,"grantor_entity_id")){cross=false;Add(e,"SETTLEMENT_DIRECTION_INVALID");}
   if(B(vr,"realized_external")&&(S(vr,"settlement_id")!=S(st,"settlement_id")||!B(st,"verified_external")||I(vr,"amount_minor")>I(st,"amount_minor"))){cross=false;Add(e,"VALUE_EXCEEDS_SETTLEMENT");}
   if(S(d,"asset_id")!=S(tr,"asset_id")||S(d,"usage_id")!=S(u,"usage_id")||S(d,"licence_id")!=S(l,"licence_id")||S(d,"settlement_id")!=S(st,"settlement_id")){cross=false;Add(e,"COMMODITY_LINK_MISMATCH");}if(I(d,"contribution_minor")>I(st,"amount_minor")){cross=false;Add(e,"COMMODITY_EXCEEDS_SETTLEMENT");}
   if(S(ca,"share_class_id")!=S(c,"share_class_id")||S(ca,"issuance_request_id")!=S(c,"issuance_request_id")){cross=false;Add(e,"CAPITAL_AUTH_MISMATCH");}if(I(O(c["accounting"]),"debit_minor")!=I(O(c["accounting"]),"credit_minor")){cross=false;Add(e,"CAPITAL_ACCOUNTING_UNBALANCED");}
   long pos=0;foreach(var x in A(c["positions"]))pos+=I(x,"shares");if(I(c,"outstanding_before")+I(c,"shares_issued")!=I(c,"outstanding_after")||pos!=I(c,"outstanding_after")){cross=false;Add(e,"CAPITAL_SHARES_UNRECONCILED");}if(S(ss,"capital_event_id")!=S(c,"capital_event_id")){cross=false;Add(e,"SHARE_SETTLEMENT_LINK_MISMATCH");}
  }else cross=false;
  var ledgerOk=reqOk&&VerifyLedger(O(ev["event_ledger"]),e);e.Sort(StringComparer.Ordinal);var overall=rootOk&&sigOk&&reqOk&&cross&&ledgerOk&&e.Count==0;
  return new Dictionary<string,object>{{"schema","entity-cleanroom-verification-result-v1"},{"transaction_id",S(bundle,"transaction_id")},{"transaction_root_sha256",S(bundle,"transaction_root_sha256")},{"root_valid",rootOk},{"signature_valid",sigOk},{"required_sections_valid",reqOk},{"cross_links_valid",cross},{"ledger_valid",ledgerOk},{"overall_valid",overall},{"error_codes",e.ToArray()}};
 }
 static string ResultHash(Dictionary<string,object> r){return Sha(Canon(r));}
 static Dictionary<string,object> VerifyRecovery(string dir,string keyFile){
  var m=O(Load(Path.Combine(dir,"RECOVERY_MANIFEST.json")));var bundle=File.ReadAllBytes(Path.Combine(dir,"TRANSACTION_BUNDLE.json"));var enc=File.ReadAllBytes(Path.Combine(dir,"STATE_BACKUP.enc"));var key=Enumerable.Range(0,File.ReadAllText(keyFile).Trim().Length/2).Select(x=>Convert.ToByte(File.ReadAllText(keyFile).Trim().Substring(x*2,2),16)).ToArray();
  var unsigned=new Dictionary<string,object>(m);unsigned.Remove("signature");var sg=O(m["signature"]);var sigOk=false;
  try{var pub=(Ed25519PublicKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(S(sg,"public_key_spki_der_b64")));var signer=new Ed25519Signer();signer.Init(false,pub);var msg=Encoding.UTF8.GetBytes(Canon(unsigned));signer.BlockUpdate(msg,0,msg.Length);sigOk=signer.VerifySignature(Convert.FromBase64String(S(sg,"sig_b64")));}catch{}
  var hashes=Sha(bundle)==S(m,"transaction_bundle_sha256")&&Sha(enc)==S(m,"encrypted_state_sha256")&&Sha(key)==S(m,"recovery_key_fingerprint_sha256");var dec=false;var restored="";
  try{var nonce=Convert.FromBase64String(S(m,"nonce_b64"));var cipher=new GcmBlockCipher(new Org.BouncyCastle.Crypto.Engines.AesEngine());cipher.Init(false,new AeadParameters(new KeyParameter(key),(int)I(m,"tag_bytes")*8,nonce));var output=new byte[cipher.GetOutputSize(enc.Length)];var len=cipher.ProcessBytes(enc,0,enc.Length,output,0);len+=cipher.DoFinal(output,len);var plain=output.Take(len).ToArray();var state=O(JS.DeserializeObject(Encoding.UTF8.GetString(plain)));restored=Sha(Canon(state["evidence"]));dec=true;}catch{}
  var ok=sigOk&&hashes&&dec&&restored==S(m,"transaction_root_sha256");return new Dictionary<string,object>{{"schema","entity-cleanroom-recovery-result-v1"},{"signature_valid",sigOk},{"hashes_valid",hashes},{"decrypt_valid",dec},{"restored_transaction_root_sha256",restored},{"expected_transaction_root_sha256",S(m,"transaction_root_sha256")},{"overall_valid",ok}};
 }
 static int Main(string[] args){
  var root=Directory.GetCurrentDirectory();if(args.Length>0&&args[0]!="test"){var r=Verify(O(Load(args[0])));Console.WriteLine(JS.Serialize(new Dictionary<string,object>{{"result_sha256",ResultHash(r)},{"result",r}}));return (bool)r["overall_valid"]?0:1;}
  var kit=Path.Combine(root,"conformance-kit");var manifest=O(Load(Path.Combine(kit,"vectors","VECTOR_MANIFEST.json")));var vectors=A(manifest["vectors"]);var rows=new ArrayList();var passed=0;
  foreach(var vv in vectors){var v=O(vv);var r=Verify(O(Load(Path.Combine(kit,"vectors",S(v,"file")))));var exp=O(v["expected"]);var ok=(bool)r["overall_valid"]==(bool)exp["overall_valid"]&&Canon(r["error_codes"])==Canon(exp["error_codes"]);if(ok)passed++;rows.Add(new Dictionary<string,object>{{"name",S(v,"name")},{"ok",ok},{"result_sha256",ResultHash(r)},{"result",r}});}
  var rec=VerifyRecovery(Path.Combine(kit,"vectors","recovery"),Path.Combine(kit,"vectors","test_inputs","recovery_key.hex"));var report=new Dictionary<string,object>{{"implementation","csharp"},{"vectors_passed",passed},{"vectors_total",vectors.Length},{"recovery_pass",(bool)rec["overall_valid"]},{"golden_root",manifest["valid_transaction_root_sha256"]},{"results",rows.ToArray()},{"recovery",rec}};Console.WriteLine(JS.Serialize(report));return passed==vectors.Length&&(bool)rec["overall_valid"]?0:1;
 }
}
