namespace Comdata.AppSupport.PPOLTestFileGenerator.Model
{
    public class ShippingMethod 
    {
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsSelected { get; set; }

        public ShippingMethod (string desciption, string code, bool isSeleleted)
        {
            this.Description = desciption;
            this.Code = code;
            this.IsSelected = isSeleleted;
        }
    }
}
