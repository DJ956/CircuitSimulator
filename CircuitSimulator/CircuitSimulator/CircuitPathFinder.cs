using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CircuitPathFinder
    {
        public CircuitPathFinder()
        {

        }

        public void FindPath(List<CircleData> circles)
        {
            //演算順序
            var order = new List<int>();

            //var inputs = CircleInputs.Inputs;
            var outsideInputs = CircleOutSizeInputs.OutSideInputs;
            var outsideOutputs = CircleOutsideOutputs.OutsideOutputs;
            var patternes = CirclePatternes.Patternes;

            //1 1 0 0 0
            circles[0].Value = true;
            circles[0].Already = true;

            circles[1].Value = true;
            circles[1].Already = true;

            circles[2].Value = false;
            circles[2].Already = true;

            circles[3].Value = false;
            circles[3].Already = true;

            circles[4].Value = false;
            circles[4].Already = true;

            for (int i = 0; i < circles.Count; i++)
            {
                var type = circles[i].CircuitType;

                switch (type)
                {
                    //枝
                    case CircuitType.branch:
                        {
                            var index = circles[i].Inputs[0];
                            if (circles[index].Already)
                            {
                                circles[i].Value = circles[index].Value;
                                circles[i].Already = true;
                            }
                            break;
                        }
                    //AND
                    case CircuitType.AND:
                        {
                            var inputsIndexes = circles[i].Inputs;//入力のindexたち
                            var inputsValues = new bool[inputsIndexes.Length]; //入力のindexたちが保有するvalue値
                            for (int j = 0; j < inputsValues.Length; j++)
                            {
                                var index = inputsIndexes[j];
                                if (circles[index].Already)
                                {
                                    inputsValues[j] = circles[j].Value;
                                }
                            }

                            circles[i].Value = CircuitFunction.AND(inputsValues);
                            circles[i].Already = true;

                            break;
                        }
                    //OR
                    case CircuitType.OR:
                        {
                            var inputsIndexes = circles[i].Inputs;//入力のindexたち
                            var inputsValue = new bool[inputsIndexes.Length]; //入力のindexたちが保有するvalue値
                            for (int j = 0; j < inputsIndexes.Length; j++)
                            {
                                var index = inputsIndexes[j];
                                if (circles[index].Already)
                                {
                                    inputsValue[j] = circles[j].Value;
                                }
                            }

                            circles[i].Value = CircuitFunction.OR(inputsValue);
                            circles[i].Already = true;
                            break;
                        }
                    //NOT
                    case CircuitType.NOT:
                        {
                            var index = circles[i].Inputs[0];
                            if (circles[index].Already)
                            {
                                circles[i].Value = CircuitFunction.NOT(circles[index].Value);
                                circles[i].Already = true;
                            }
                            break;
                        }
                    //NAND
                    case CircuitType.NAND:
                        {
                            var inputsIndexes = circles[i].Inputs;//入力のindexたち
                            var inputsValue = new bool[inputsIndexes.Length]; //入力のindexたちが保有するvalue値
                            for (int j = 0; j < inputsIndexes.Length; j++)
                            {
                                var index = inputsIndexes[j];
                                if (circles[index].Already)
                                {
                                    inputsValue[j] = circles[j].Value;
                                }
                            }

                            circles[i].Value = CircuitFunction.NAND(inputsValue);
                            circles[i].Already = true;
                            break;
                        }
                    case CircuitType.PO:
                        {
                            var index = circles[i].Inputs[0];
                            if (circles[index].Already)
                            {
                                circles[i].Value = circles[index].Value;
                                circles[i].Already = true;
                            }
                            break;
                        }
                }
            }


            Console.WriteLine("---------------------------------");

            foreach (var c in circles)
            {
                Console.WriteLine(c.ToString());
            }

        }
    }
}
