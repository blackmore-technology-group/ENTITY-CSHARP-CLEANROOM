using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
const string KitPath="passport-conformance-kit/ENTITY_V3_4_GLOBAL_PASSPORT_CLEANROOM_KIT.min.json";
const string KitSha="5869a3fd0ed6cb9f65bf4b20c3bd64933cad82f4aef05c5809e2e05af921f230";
const string Expected="ac7504cce70576008cff069607619660a4b9bf0cad43b3f3de81078f1e80d9ba";
string[] Core={"ENTITY","AUTHORITY","RIGHT","EVENT","VALUE"};
string Sha(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
string S(JsonObject r,string k)=>r[k]?.GetValue<string>()??"";
bool B(JsonObject r,string k,bool w)=>r[k] is JsonValue v&&v.TryGetValue<bool>(out var x)&&x==w;
bool Hex64(JsonNode? n)=>n is JsonValue v&&v.TryGetValue<string>(out var s)&&s.Length==64&&s.All(c=>char.IsDigit(c)||(c>='a'&&c<='f'));
bool ArrEq(JsonNode? n,string[] w)=>n is JsonArray a&&a.Count==w.Length&&a.Select(x=>x?.GetValue<string>()??"").SequenceEqual(w);
bool Stack(JsonObject r){if(r["profile_refs"] is not JsonArray refs||refs.Count==0||!refs.Any(x=>x?.GetValue<string>()=="entity-profile:global@1.0"))return false;if(r["profile_hashes"] is not JsonArray hs||hs.Count!=refs.Count||!hs.All(Hex64))return false;return B(r,"fail_closed",true)&&B(r,"profile_composition_does_not_create_authority",true)&&B(r,"standards_mapping_is_not_normative_equivalence",true);}
bool Valid(JsonObject r)=>S(r,"schema") switch{
"entity-v3-global-passport-profile-status-v1"=>ArrEq(r["core_primitives"],Core)&&B(r,"core_semantics_changed",false)&&B(r,"market_engine_preserved",true)&&B(r,"one_passport_many_profiles",true)&&B(r,"evidence_truth_boundary_preserved",true),
"entity-v3-global-profile-v1"=>S(r,"profile_ref").Length>0&&new[]{"GLOBAL","JURISDICTION","INDUSTRY","DOMAIN","PRIVACY","TRUST","DISCLOSURE"}.Contains(S(r,"kind"))&&Hex64(r["schema_sha256"])&&B(r,"profile_is_not_authority",true)&&B(r,"standards_mapping_is_not_normative_equivalence",true)&&(!S(r,"profile_id").ToUpperInvariant().Contains("DEFENCE")||B(r,"public_unclassified",true)),
"entity-v3-profile-stack-resolution-v1"=>Stack(r),
"entity-v3-global-passport-v1"=>ArrEq(r["core_primitives"],Core)&&S(r,"rights_passport_id").Length>0&&Hex64(r["rights_passport_sha256"])&&r["profile_stack"] is JsonObject ps&&Stack(ps)&&B(r,"one_passport_many_profiles",true)&&B(r,"profile_composition_does_not_create_authority",true)&&B(r,"standards_mapping_is_not_normative_equivalence",true)&&B(r,"evidence_does_not_establish_objective_truth",true)&&B(r,"legal_effect_is_deployment_specific",true)&&B(r,"underlying_information_remains_nonrival",true)&&r["economic_state"] is JsonObject e&&e["amount_units"] is JsonValue av&&av.TryGetValue<long>(out var amount)&&amount>=0&&B(e,"market_observation_is_not_accounting_fair_value",true)&&r["standards_mappings"] is JsonArray ms&&ms.All(x=>x is JsonObject m&&B(m,"normative_equivalence_claimed",false)),
"entity-v3-continuous-ingest-result-v1"=>r["files"] is JsonValue fv&&fv.TryGetValue<long>(out var files)&&files>=0&&Hex64(r["inventory_sha256"])&&B(r,"content_addressed",true)&&B(r,"custody_is_not_authority",true)&&B(r,"economic_value_invented",false),
_=>false};
string Canon(JsonNode? n){if(n is null)return"null";if(n is JsonObject o)return"{"+string.Join(",",o.OrderBy(k=>k.Key,StringComparer.Ordinal).Select(k=>JsonSerializer.Serialize(k.Key)+":"+Canon(k.Value)))+"}";if(n is JsonArray a)return"["+string.Join(",",a.Select(Canon))+"]";return n.ToJsonString(new JsonSerializerOptions{WriteIndented=false});}
var raw=File.ReadAllBytes(KitPath);if(Sha(raw)!=KitSha)throw new Exception("sealed v3.4 kit SHA-256 mismatch");var kit=JsonNode.Parse(raw)!.AsObject();var cases=kit["cases"]!.AsArray().Select(x=>x!.AsObject()).OrderBy(x=>S(x,"id"),StringComparer.Ordinal).ToArray();var transcript=new JsonArray();int passed=0;foreach(var c in cases){var actual=Valid(c["record"]!.AsObject())?"VALID":"INVALID";if(actual==S(c,"expect"))passed++;transcript.Add(new JsonObject{{"id",S(c,"id")},{"actual",actual}});}var result=Sha(Encoding.UTF8.GetBytes(Canon(transcript)));var overall=passed==24&&result==Expected&&kit["expected_result_sha256"]?.GetValue<string>()==Expected;Console.WriteLine(JsonSerializer.Serialize(new{implementation="csharp",kit_sha256=KitSha,vectors_passed=passed,vectors_total=24,result_sha256=result,expected_result_sha256=Expected,overall_valid=overall},new JsonSerializerOptions{WriteIndented=true}));return overall?0:1;
