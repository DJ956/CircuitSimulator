using System;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class CircuitPathFinder
    {
        public CircuitPathFinder() { }

        /// <summary>
        /// 入力パターン選択
        /// </summary>
        /// <param name="circles"></param>
        /// <param name="patternes"></param>
        /// <param name="selectPatternIndex"></param>
        private void Initialize(List<CircleData> circles, CirclePatternes circlePatternes, int selectPatternIndex)
        {
            //回路データの値の初期化
            foreach(var c in circles)
            {
                c.Already = false;
                c.Value = false;
            }

            //外部入力値設定
            for (int i = 0; i < circlePatternes.Patternes[selectPatternIndex].Count; i++)
            {
                if (circlePatternes.Patternes[selectPatternIndex][i] == 0)
                {
                    circles[i].Value = false;
                }
                else
                {
                    circles[i].Value = true;
                }
                circles[i].Already = true;
            }
            /*
            Console.WriteLine("Pattern = ");
            patternes[selectPatternIndex].ForEach(p => { Console.Write(p + " "); });
            Console.WriteLine("");
            */
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
                    //出力
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
                        throw new Exception("Not found Gate process");                        
                    }
            }
        }

        /// <summary>
        /// 論理シミュレータを実行して、入力パターンから出力を得る。
        /// </summary>
        /// <param name="circles">回路データリスト</param>
        /// <param name="patternes">入力パターン</param>
        /// <param name="selectPatternIndex">パターンの選択番号</param>
        /// <returns>論理回路の出力結果</returns>
        public List<CircleData> Simulation(List<CircleData> circles, CirclePatternes circlePatternes, int selectPatternIndex)
        {     
            Initialize(circles, circlePatternes, selectPatternIndex);
           
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
