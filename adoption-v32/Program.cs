using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

const string KitPath="adoption-conformance-kit/ENTITY_V3_2_ADOPTION_CLEANROOM_KIT.min.json";
const string KitSha="44e7a00f910c89aced3b3c1b5e9cba486809ca313e9bfb4b7bc9266095c10c14";
const string Expected="1eb59e09ab08da86bfd8584df4a64ba331f7bbbce3d236b9b94f351606c90e18";
string[] Primitives={"ENTITY","AUTHORITY","RIGHT","EVENT","VALUE"};
string[] Lifecycle={"DCO","INSTRUMENT","LISTING","DISCLOSURE","ORDER_RFQ_AUCTION","PRICE_DISCOVERY","TRADE","CLEARING","SETTLEMENT","ENTITLEMENT","USAGE","DERIVED_OUTPUT","ECONOMIC_CONSEQUENCE"};
var Providers=new HashSet<string>{"AWS_S3","AZURE_BLOB","GOOGLE_CLOUD_STORAGE","SNOWFLAKE","DATABRICKS","POSTGRESQL","SQL_SERVER","LOCAL_FILESYSTEM","HTTP_API"};
var Standards=new HashSet<string>{"ODRL","W3C_VC","DID","GAIA_X","IDS"};
string Sha(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
bool Hex64(JsonNode? n)=>n is JsonValue j&&j.TryGetValue<string>(out var s)&&s.Length==64&&s.All(c=>char.IsDigit(c)||(c>='a'&&c<='f'));
string S(JsonObject r,string k)=>r[k]?.GetValue<string>()??"";
bool B(JsonObject r,string k,bool want)=>r[k] is JsonValue v&&v.TryGetValue<bool>(out var b)&&b==want;
bool ArrEq(JsonNode? n,string[] want)=>n is JsonArray a&&a.Count==want.Length&&a.Select(x=>x?.GetValue<string>()??"").SequenceEqual(want);
bool Rules(JsonNode? n){if(n is not JsonArray a||a.Count==0)return false;foreach(var x in a){if(x is not JsonObject q)return false;var e=S(q,"effect");if(e is not("ALLOW" or "REQUIRE" or "PROHIBIT"))return false;if(q["actions"] is not JsonArray aa||aa.Count==0)return false;var xs=aa.Select(y=>y?.GetValue<string>()??"").ToArray();if(xs.Any(z=>z.Length==0||z!=z.ToUpperInvariant())||!xs.SequenceEqual(xs.Distinct().OrderBy(z=>z,StringComparer.Ordinal)))return false;}return true;}
bool Valid(JsonObject r)=>S(r,"schema") switch{
"entity-v3-rights-passport-v1"=>ArrEq(r["core_primitives"],Primitives)&&Rules(r["rights"])&&B(r,"provider_custody_is_not_authority",true)&&B(r,"underlying_data_not_silently_transferred",true)&&B(r,"legal_effect_is_deployment_specific",true),
"entity-v3-custody-locator-v1"=>Providers.Contains(S(r,"provider"))&&Hex64(r["content_sha256"])&&B(r,"provider_is_authority",false)&&B(r,"credentials_included",false)&&B(r,"entity_identity_changes_with_provider",false),
"entity-v3-standards-mapping-v1"=>Standards.Contains(S(r,"source_standard"))&&Hex64(r["source_sha256"])&&B(r,"silent_semantic_equivalence",false)&&B(r,"external_standard_is_not_entity_authority",true),
"entity-v3-external-credential-evidence-v1"=>S(r,"source_standard")=="W3C_VC"&&Hex64(r["credential_sha256"])&&B(r,"credential_is_evidence_not_entity_authority",true),
"entity-v3-resolver-deployment-v1"=>S(r,"mode")=="FEDERATED"&&(r["minimum_resolvers"]?.GetValue<int>()??0)>=2&&B(r,"resolver_is_not_authority",true)&&B(r,"single_provider_dependency_prohibited",true)&&B(r,"fail_closed",true),
"entity-v3-exchange-adoption-profile-v1"=>B(r,"market_engine_preserved",true)&&B(r,"rights_are_traded_not_bytes",true)&&ArrEq(r["market_lifecycle"],Lifecycle),
"entity-v3-adoption-profile-status-v1"=>ArrEq(r["core_primitives"],Primitives)&&B(r,"core_semantics_changed",false)&&B(r,"market_engine_preserved",true),
"entity-v3-legal-classification-assertion-v1"=>S(r,"asserted_by").Length>0&&S(r,"classification").Length>0&&B(r,"classification_is_assertion_not_protocol_legal_truth",true),
_=>false};
string Canon(JsonNode? n){if(n is null)return"null";if(n is JsonObject o)return"{"+string.Join(",",o.OrderBy(k=>k.Key,StringComparer.Ordinal).Select(k=>JsonSerializer.Serialize(k.Key)+":"+Canon(k.Value)))+"}";if(n is JsonArray a)return"["+string.Join(",",a.Select(Canon))+"]";return n.ToJsonString(new JsonSerializerOptions{WriteIndented=false});}
var raw=File.ReadAllBytes(KitPath);if(Sha(raw)!=KitSha)throw new Exception("sealed kit SHA-256 mismatch");var kit=JsonNode.Parse(raw)!.AsObject();var rows=new JsonArray();int passed=0;foreach(var item in kit["vectors"]!.AsArray()){var v=item!.AsObject();bool accepted=Valid(v["record"]!.AsObject());string exp=S(v,"expect");bool ok=accepted==(exp=="VALID");if(ok)passed++;rows.Add(new JsonObject{{"name",S(v,"name")},{"accepted",accepted},{"expected",exp},{"ok",ok}});}var sorted=new JsonArray(rows.Select(x=>x!.DeepClone()).OrderBy(x=>x!["name"]!.GetValue<string>(),StringComparer.Ordinal).ToArray());var summary=new JsonObject{{"schema","entity-v3.2-adoption-cleanroom-result-v1"},{"profile","ENTITY-ADOPTION-LAYER"},{"adoption_invariants",kit["profile"]!["adoption_invariants"]!.DeepClone()},{"vectors",sorted}};string result=Sha(Encoding.UTF8.GetBytes(Canon(summary)));bool overall=passed==16&&result==Expected;Console.WriteLine(JsonSerializer.Serialize(new{implementation="csharp",kit_sha256=KitSha,vectors_passed=passed,vectors_total=16,result_sha256=result,expected_result_sha256=Expected,overall_valid=overall},new JsonSerializerOptions{WriteIndented=true}));return overall?0:1;
