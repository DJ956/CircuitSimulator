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

        /// <summary>
        /// 入力パターン選択
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="patternes"></param>
        /// <param name="selectPatternIndex"></param>
        private void Initialize(List<CircleData> circles, List<List<int>> patternes, int selectPatternIndex)
        {
            for (int i = 0; i < patternes[selectPatternIndex].Count; i++)
            {
                if (patternes[selectPatternIndex][i] == 0)
                {
                    circles[i].Value = false;
                }
                else
                {
                    circles[i].Value = true;
                }
                circles[i].Already = true;
            }
            Console.WriteLine("Pattern = ");
            patternes[selectPatternIndex].ForEach(p => { Console.Write(p + " "); });
            Console.WriteLine("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="i"></param>
        private void FindPath(List<CircleData> circles, int i)
        {
            var type = circles[i].CircuitType;

            switch (type)
            {
                case CircuitType.PI:
                    {
                        break;
                    }
                //枝
                case CircuitType.branch:
                    {
                        var index = circles[i].Inputs[0] - 1;
                        if (circles[index].Already)
                        {
                            circles[i].Value = circles[index].Value;
                            circles[i].Already = true;
                        }
                        else
                        {
                            //FindPath(circles, index);
                            Console.WriteLine($"Not Already:{index+1} Occuresed:{i+1} Branch");
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
                            var index = inputsIndexes[j] - 1;
                            if (circles[index].Already)
                            {
                                inputsValues[j] = circles[index].Value;
                            }
                            else
                            {
                                //FindPath(circles, index);
                                Console.WriteLine($"Not Already:{index+1} Occuresed:{i+1} AND");
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
                            var index = inputsIndexes[j] - 1;
                            if (circles[index].Already)
                            {
                                inputsValue[j] = circles[index].Value;
                            }
                            else
                            {
                                //FindPath(circles, index);
                                Console.WriteLine($"Not Already:{index+1} Occuresed:{i+1} OR");
                            }
                        }

                        circles[i].Value = CircuitFunction.OR(inputsValue);
                        circles[i].Already = true;
                        break;
                    }
                //NOT
                case CircuitType.NOT:
                    {
                        var index = circles[i].Inputs[0] - 1;
                        if (circles[index].Already)
                        {
                            circles[i].Value = CircuitFunction.NOT(circles[index].Value);
                            circles[i].Already = true;
                        }
                        else
                        {
                            //FindPath(circles, index);
                            Console.WriteLine($"Not Already:{index+1} Occuresed:{i+1} NOT");
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
                            var index = inputsIndexes[j] - 1;                            
                            if (circles[index].Already)
                            {
                                inputsValue[j] = circles[index].Value;
                            }
                            else
                            {
                                //FindPath(circles, index);
                                Console.WriteLine($"Not Already:{index+1} Occuresed:{i+1} NAND");
                            }
                        }

                        circles[i].Value = CircuitFunction.NAND(inputsValue);
                        circles[i].Already = true;
                        break;
                    }
                case CircuitType.PO:
                    {
                        var index = circles[i].Inputs[0] - 1;
                        if (circles[index].Already)
                        {
                            circles[i].Value = circles[index].Value;
                            circles[i].Already = true;
                        }
                        else
                        {
                            //FindPath(circles, index);
                            Console.WriteLine($"Not Already:{index+1} Occuresed:{i+1} PO");
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Exception");
                        break;
                    }
            }
        }

        /// <summary>
        /// 論理シミュレータを実行して、入力パターンから出力を得る。
        /// </summary>
        /// <param name="circles"></param>
        /// <returns></returns>
        public List<CircleData> FindPath(List<CircleData> circles)
        {
            
            //var inputs = CircleInputs.Inputs;
            var outsideInputs = CircleOutSizeInputs.OutSideInputs;
            var outsideOutputs = CircleOutsideOutputs.OutsideOutputs;          
            Initialize(circles, CirclePatternes.Patternes, 0);
           
            for (int i = 0; i < circles.Count; i++)
            {
                FindPath(circles, i);
            }
            
            //出力のみを取得する
            var result = new List<CircleData>();
            foreach(var c in circles)
            {
                if(c.CircuitType == CircuitType.PO)
                {
                    result.Add(c);
                }
            }

            result.Sort((a, b) => a.Inputs[0] - b.Inputs[0]);
            return result;
        }
    }
}
