using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Enumerates the styles of activation functions available.
    /// </summary>
    public enum ActivationStyle
    {
        SigmoidBasic = 0,
        SigmoidNEAT = 1,
        ReLU = 2,
        None = 3
    }

    public static class ActivationFunctions
    {
        /// <summary>
        /// Holds relative weighted chance of selecting an activation style.
        /// </summary>
        public static Dictionary<ActivationStyle, int> ActivationStyleWeights = new Dictionary<ActivationStyle, int>()
        {
            {ActivationStyle.SigmoidBasic, 0 },
            {ActivationStyle.SigmoidNEAT, 1 },
            {ActivationStyle.ReLU, 0 },
            {ActivationStyle.None, 0 }
        };

        /// <summary>
        /// Chooses a random activation style based on the weights of each style.
        /// </summary>
        /// <param name="rando">A random number generator.</param>
        /// <returns>A random activation style. ActivationStyle.SigmoidBasic by default.</returns>
        public static ActivationStyle ChooseActivationStyle(Random rando)
        {
            int total = 0;
            foreach (var item in ActivationStyleWeights)
            {
                total += item.Value;
            }
            int pos = rando.Next(total);

            foreach (var style in ActivationStyleWeights)
            {
                pos -= style.Value;
                if (pos < 0)
                {
                    return style.Key;
                }
            }

            return ActivationStyle.SigmoidBasic;
        }
        
        /// <summary>
        /// Retrieves the activation function from the given style.
        /// </summary>
        /// <param name="style">The style corresponding to the function.</param>
        /// <returns>The desired activation function.</returns>
        public static Func<double,double> GetActivationFunction(ActivationStyle style)
        {
            switch (style)
            {
                case ActivationStyle.SigmoidBasic:
                    return Sigmoid;
                case ActivationStyle.SigmoidNEAT:
                    return SigmoidNEAT;
                case ActivationStyle.ReLU:
                    return ReLU;
                case ActivationStyle.None:
                    return UnitLine;
                default:
                    throw new Exception("ActivationFunctions.GetActivationFunction style not found.");
                    break;
            }

            return Sigmoid;
        }

        /// <summary>
        /// The basic sigmoid function.
        /// </summary>
        /// <param name="x">The input.</param>
        /// <returns>Sigmoid function.</returns>
        public static double Sigmoid(double x)
        {
            double denominator = 1 + Math.Exp(-x);
            return 1 / denominator;
        }
        
        /// <summary>
        /// The sigmoid function used in the original NEAT algorithm.
        /// </summary>
        /// <param name="x">The input.</param>
        /// <returns>Modified sigmoid function.</returns>
        public static double SigmoidNEAT(double x)
        {
            double denominator = 1 + Math.Exp(-4.924273 * x);
            return 1 / denominator;
        }

        /// <summary>
        /// Basic ReLU activation function.
        /// </summary>
        /// <param name="x">Input value of function.</param>
        /// <returns>The max of 'x' and 0.</returns>
        public static double ReLU(double x)
        {
            return Math.Max(x, 0);
        }

        /// <summary>
        /// Straight up y = x
        /// </summary>
        /// <param name="x">Input value of function.</param>
        /// <returns>The value x.</returns>
        public static double UnitLine(double x)
        {
            return x;
        }

    }
}
