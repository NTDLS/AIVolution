namespace Determinet.ActivationFunctions
{
    public interface IActivationFunction
    {
        double Activation(double x);
        double Derivative(double x);
    }
}
