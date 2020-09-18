namespace CognitiveSearch.UI.Services.ARM
{
    class ArmResult
    {
        public ArmTenant[] value {get; set;}
    }   
    class ArmTenant
    {
        public string tenantId { get; set; }
    }
}
