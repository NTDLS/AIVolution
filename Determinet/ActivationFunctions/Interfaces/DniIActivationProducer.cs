namespace Determinet.ActivationFunctions.Interfaces
{
    /// <summary>
    /// These activation functions have a Activation() and Derivative() but they also have a Produce()
    /// function which used to create the final value for whch the product is independent of training.
    /// </summary>
    public interface DniIActivationProducer : DniIFunction
    {
        double Activation(double x);
        double Derivative(double x);
        double Produce(double x);
    }
}
