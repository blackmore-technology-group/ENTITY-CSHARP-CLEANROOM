using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
const string KitPath="reality-conformance-kit/ENTITY_V3_3_REALITY_CLEANROOM_KIT.min.json";
const string KitSha="e0d6ba26baa405557bc2990e39d3022ebb8cda00ae797fab0100773cf304a6fd";
const string Expected="82bd1f1fb328edd37a26d8ea60ede5a599c7d9af5027bffd73b9e52843b5a51d";
string[] P={"ENTITY","AUTHORITY","RIGHT","EVENT","VALUE"};
var States=new HashSet<string>{"OBSERVED","ASSERTED","INFERRED","ATTESTED","EXTERNALLY_VERIFIED","ADJUDICATED","DISPUTED","REVOKED","UNKNOWN"};
var Evidence=new HashSet<string>{"SENSOR_OBSERVATION","DOCUMENT","REGISTRY_RECORD","LAB_RESULT","PAYMENT_RECORD","IMAGE","API_RESPONSE","CERTIFICATE","COURT_RECORD","OTHER"};
var Anchors=new HashSet<string>{"GOVERNMENT_REGISTRY","SENSOR_NETWORK","BANK_SETTLEMENT","LAB_SYSTEM","SUPPLY_CHAIN_SYSTEM","CORPORATE_REGISTRY","COURT_RECORD","CERTIFICATE_AUTHORITY","OTHER"};
var Nodes=new HashSet<string>{"SOURCE_DATA","DCO","RIGHT","LICENSE","USAGE","DERIVED_ASSET","PRODUCT","TRANSACTION","REVENUE","SETTLEMENT","CONTRIBUTOR"};
var Edges=new HashSet<string>{"ORIGINATED_FROM","AUTHORIZED_BY","LICENSED_AS","USED_IN","DERIVED_FROM","PRODUCED","GENERATED","SETTLED_AS","CONTRIBUTED_TO"};
string Sha(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
string S(JsonObject r,string k)=>r[k]?.GetValue<string>()??"";
bool B(JsonObject r,string k,bool w)=>r[k] is JsonValue v&&v.TryGetValue<bool>(out var x)&&x==w;
bool Hex64(JsonNode? n)=>n is JsonValue v&&v.TryGetValue<string>(out var s)&&s.Length==64&&s.All(c=>char.IsDigit(c)||(c>='a'&&c<='f'));
bool Refs(JsonNode? n,bool nonempty=false){if(n is not JsonArray a||nonempty&&a.Count==0)return false;var xs=a.Select(x=>x?.GetValue<string>()??"").ToArray();return xs.All(x=>x.Length>0)&&xs.SequenceEqual(xs.Distinct().OrderBy(x=>x,StringComparer.Ordinal));}
bool ArrEq(JsonNode? n,string[] w)=>n is JsonArray a&&a.Count==w.Length&&a.Select(x=>x?.GetValue<string>()??"").SequenceEqual(w);
bool Valid(JsonObject r)=>S(r,"schema") switch{
"entity-v3-evidence-object-v1"=>Evidence.Contains(S(r,"evidence_type"))&&Hex64(r["content_sha256"])&&B(r,"signature_proves_attribution_not_objective_truth",true)&&B(r,"immutable_evidence_record",true),
"entity-v3-evidence-bound-claim-v1"=>States.Contains(S(r,"state"))&&Hex64(r["value_sha256"])&&Refs(r["evidence_refs"])&&B(r,"claim_is_not_objective_truth",true)&&B(r,"state_is_typed_not_absolute",true),
"entity-v3-claim-status-transition-v1"=>States.Contains(S(r,"from_state"))&&States.Contains(S(r,"to_state"))&&S(r,"from_state")!=S(r,"to_state")&&Refs(r["evidence_refs"])&&B(r,"history_rewrite_prohibited",true)&&B(r,"transition_does_not_establish_objective_truth",true),
"entity-v3-attestation-authority-grant-v1"=>Refs(r["scopes"],true)&&Hex64(r["authority_evidence_sha256"])&&B(r,"attestation_authority_is_scope_limited",true)&&B(r,"attestation_does_not_create_legal_truth",true),
"entity-v3-attestation-v1"=>S(r,"grant_id").Length>0&&S(r,"scope").Length>0&&Refs(r["evidence_refs"],true)&&B(r,"attestation_is_evidence_not_objective_truth",true),
"entity-v3-external-reality-anchor-v1"=>Anchors.Contains(S(r,"anchor_type"))&&Hex64(r["endpoint_descriptor_sha256"])&&B(r,"credentials_included",false)&&B(r,"external_system_is_not_automatic_entity_authority",true),
"entity-v3-external-reality-snapshot-v1"=>Hex64(r["record_sha256"])&&Refs(r["verifier_evidence_refs"])&&B(r,"external_record_is_evidence_not_protocol_truth",true)&&B(r,"record_may_be_contested_or_superseded",true),
"entity-v3-causal-economic-node-v1"=>Nodes.Contains(S(r,"node_type"))&&Refs(r["evidence_refs"])&&Refs(r["event_refs"])&&((r["economic_observation"] is not JsonObject o)||o.Count==0||(B(o,"market_observation_is_not_accounting_fair_value",true)&&B(o,"protocol_does_not_determine_legal_entitlement",true))),
"entity-v3-causal-economic-edge-v1"=>Edges.Contains(S(r,"edge_type"))&&S(r,"from_node_id")!=S(r,"to_node_id")&&Refs(r["evidence_refs"],true)&&Refs(r["authority_refs"])&&Refs(r["participation_rule_refs"])&&B(r,"causality_is_evidence_bound_not_assumed",true)&&B(r,"economic_attribution_is_not_accounting_fair_value",true),
"entity-v3-verifiable-reality-status-v1"=>ArrEq(r["core_primitives"],P)&&B(r,"core_semantics_changed",false)&&B(r,"market_engine_preserved",true)&&B(r,"reality_claims_are_evidence_bound",true)&&B(r,"cryptographic_verification_is_not_objective_truth",true)&&B(r,"protocol_verification_is_not_objective_truth",true),
_=>false};
string Canon(JsonNode? n){if(n is null)return"null";if(n is JsonObject o)return"{"+string.Join(",",o.OrderBy(k=>k.Key,StringComparer.Ordinal).Select(k=>JsonSerializer.Serialize(k.Key)+":"+Canon(k.Value)))+"}";if(n is JsonArray a)return"["+string.Join(",",a.Select(Canon))+"]";return n.ToJsonString(new JsonSerializerOptions{WriteIndented=false});}
var raw=File.ReadAllBytes(KitPath);if(Sha(raw)!=KitSha)throw new Exception("sealed v3.3 kit SHA-256 mismatch");var kit=JsonNode.Parse(raw)!.AsObject();var cases=kit["cases"]!.AsArray().Select(x=>x!.AsObject()).OrderBy(x=>S(x,"id"),StringComparer.Ordinal).ToArray();var transcript=new JsonArray();int passed=0;foreach(var c in cases){var actual=Valid(c["record"]!.AsObject())?"VALID":"INVALID";if(actual==S(c,"expect"))passed++;transcript.Add(new JsonObject{{"id",S(c,"id")},{"actual",actual}});}var result=Sha(Encoding.UTF8.GetBytes(Canon(transcript)));var overall=passed==20&&result==Expected&&kit["expected_result_sha256"]?.GetValue<string>()==Expected;Console.WriteLine(JsonSerializer.Serialize(new{implementation="csharp",kit_sha256=KitSha,vectors_passed=passed,vectors_total=20,result_sha256=result,expected_result_sha256=Expected,overall_valid=overall},new JsonSerializerOptions{WriteIndented=true}));return overall?0:1;
