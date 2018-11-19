using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public static class Pole1
    {
        const double GRAVITY = 9.8;
        const double MASSCART = 1.0;
        const double MASSPOLE = 0.1;
        const double TOTAL_MASS = (MASSPOLE + MASSCART);
        const double LENGTH = 0.5;     /* actually half the pole's length */
        const double POLEMASS_LENGTH = (MASSPOLE * LENGTH);
        const double FORCE_MAG = 10.0;
        const double TAU = 0.02;   /* seconds between state updates */
        const double FOURTHIRDS = 1.3333333333333;
        const double twelve_degrees = 0.2094384;

        public static void Pole1_Evaluate(Random rando, Network network, GenomeFitnessInformation fitnessInfo)
        {

            int maxSteps = 100000;
            int steps = 0;
            int y;

            Dictionary<int, double> inputArray = new Dictionary<int, double>()
            {
                {1, 0.0 },
                {2, 0.0 },
                {3, 0.0 },
                {4, 0.0 },
            };

            CartInfo cartInfo = new CartInfo(rando);

            if (false)
            {
                inputArray[1] = (cartInfo.x + 2.4) / 4.8; ;
                inputArray[2] = (cartInfo.x_Dot + .75) / 1.5;
                inputArray[3] = (cartInfo.theta + twelve_degrees) / .41;
                inputArray[4] = (cartInfo.theta_Dot + 1.0) / 2.0;
                network.LoadSensors(inputArray);
                network.Initialize();
            }
            
            while (steps++ < maxSteps)
            {
                inputArray[1] = (cartInfo.x + 2.4) / 4.8;
                inputArray[2] = (cartInfo.x_Dot + .75) / 1.5;
                inputArray[3] = (cartInfo.theta + twelve_degrees) / .41;
                inputArray[4] = (cartInfo.theta_Dot + 1.0) / 2.0;
                network.LoadSensors(inputArray);
                
                Dictionary<int,double> outputArray = network.Activate();

                double val5 = 0.0;
                double val6 = 0.0;
                
                if (outputArray.Keys.Contains(5))
                {
                    val5 = outputArray[5];
                }
                if (outputArray.Keys.Contains(6))
                {
                    val6 = outputArray[6];
                }
                
                if (val5 > val6)
                {
                    y = 0;
                }
                else
                {
                    y = 1;
                }

                cartInfo.AdjustCart(y);

                if (!cartInfo.IsStable())
                {
                    break;
                }

            }

            fitnessInfo.Score = steps;
        }


        private class CartInfo
        {
            public double x;
            public double x_Dot;
            public double theta;
            public double theta_Dot;
            

            public CartInfo(Random rando)
            {
                x = (rando.Next(int.MaxValue) % 4800) / 1000.0 - 2.4;
                x_Dot = (rando.Next(int.MaxValue) % 2000) / 1000.0 - 1;
                theta = (rando.Next(int.MaxValue) % 400) / 1000.0 - .2;
                theta_Dot = (rando.Next(int.MaxValue) % 3000) / 1000.0 - 1.5;
            }

            public CartInfo()
            {
                x = 0;
                x_Dot = 0;
                theta = 0;
                theta_Dot = 0;
            }

            public bool IsStable()
            {
                return !(x < -2.4 || x > 2.4 || theta < -twelve_degrees || theta > twelve_degrees);
            }

            public void AdjustCart(int action)
            {
                double xacc, thetaacc, force, costheta, sintheta, temp;

                force = (action > 0) ? FORCE_MAG : -FORCE_MAG;
                costheta = Math.Cos(theta);
                sintheta = Math.Sin(theta);

                temp = (force + POLEMASS_LENGTH * theta_Dot * theta_Dot * sintheta)
                / TOTAL_MASS;

                thetaacc = (GRAVITY * sintheta - costheta * temp)
                / (LENGTH * (FOURTHIRDS - MASSPOLE * costheta * costheta
                / TOTAL_MASS));

                xacc = temp - POLEMASS_LENGTH * thetaacc * costheta / TOTAL_MASS;

                /*** Update the four state variables, using Euler's method. ***/

                x += TAU * x_Dot;
                x_Dot += TAU * xacc;
                theta += TAU * theta_Dot;
                theta_Dot += TAU * thetaacc;


            }
        }
        
    }
}
