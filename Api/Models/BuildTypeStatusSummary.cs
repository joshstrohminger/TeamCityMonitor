namespace Api.Models
{
    public class BuildTypeStatusSummary
    {
        #region Not populated by response

        public string ErrorMessage { get; set; }
        public bool IsSuccessful => ErrorMessage is null;

        #endregion

        #region Populated by response

        public string Name { get; set; }
        public string WebUrl { get; set; }
        public BuildCollection Builds { get; set; }
        public InvestigationCollection Investigations { get; set; }

        #endregion
    }
}
