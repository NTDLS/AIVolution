namespace Determinet.ActivationFunctions.Interfaces
{
    public interface DniIActivationOutputFeed : DniIActivationFunction
    {
        double[] Activation(double[] previousLayer);
    }
}
