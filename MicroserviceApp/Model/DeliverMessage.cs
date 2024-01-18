namespace Model
{
    public class DeliverMessage
    {
        public TypeOfRequest typeOfRequest;
        public Product product;

        public DeliverMessage()
        {

        }

        public DeliverMessage(TypeOfRequest typeOfRequest, Product product)
        {
            this.typeOfRequest = typeOfRequest;
            this.product = product;
        }
    }
}
