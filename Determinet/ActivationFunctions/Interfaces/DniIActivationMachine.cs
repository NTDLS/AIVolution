namespace Determinet.ActivationFunctions.Interfaces
{
    /// <summary>
    /// These activation functions have a Activation() and Derivative() but they also have a product
    /// function which used to get the final value for whch the product is independent of training.
    /// </summary>
    public interface DniIActivationMachine : DniIActivationFunction
    {
        double Produce(double x);
    }
}
