namespace Determinet.ActivationFunctions
{
    public interface IActivationMachine : IActivationFunction
    {
        double Generate(double x);
    }
}
