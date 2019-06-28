using CircuitSimulator.gate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CircuitSimulator
{
    public class CircleDataBuilder
    {
        private static readonly int TYPE_INDEX = 0;
        private static readonly int INPUT_COUNT_INDEX = 1;
        private static readonly int INPUT_INDEX = 2;
        private static readonly int OUT_COUNT_INDEX = 3;
        private static readonly int OUT_INDEX = 4;

        public CircleDataBuilder() { }

        /// <summary>
        /// 演算順序の優先度を決定する
        /// </summary>
        /// <param name="circles"></param>
        private void DetectPriority(CircleData[] circles)
        {
            var priority = new Queue<int>(Enumerable.Range(0, circles.Length));

            do
            {               
                for (int i = 0; i < circles.Length; i++)
                {
                    if (circles[i].Already) { continue; }                    
                    //PI                    
                    if (circles[i].CircuitType == CircuitType.PI)
                    {
                        circles[i].Priority = priority.Dequeue();
                        continue;
                    }

                    var inputs = circles[i].Inputs;
                    //入力が1つのみの場合
                    if (inputs.Length == 1)
                    {
                        var index = inputs[0] - 1;
                        //入力が1ならば優先順位を割り当てる
                        if (circles[index].Priority != -1) { circles[i].Priority = priority.Dequeue(); }

                    }//入力が2つ以上の場合
                    else
                    {
                        var allAlready = true;
                        //全ての入力線が演算順序決定済みか調べる。
                        for (int j = 0; j < inputs.Length; j++)
                        {
                            var index = inputs[j] - 1;
                            if (!circles[index].Already)
                            {
                                allAlready = false;
                                break;
                            }
                        }
                        //全ての入力線が順序決定済みの場合
                        if (allAlready) { circles[i].Priority = priority.Dequeue(); }
                    }

                }
            } while (priority.Count != 0);

            //最後に優先順序ごとにソートする
            Array.Sort(circles, (a, b) => a.Priority - b.Priority);
        }


        /// <summary>
        /// txtから回路情報を読み取り、Circleリストを作成する
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public CircleData[] BuildCircles(int[,] circleRaw, CircleInputs circleInputs)
        {
            var data = circleRaw;

            var result = new CircleData[data.GetLength(0)];

            var index = 1;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                var type = CircleTypeDict.GetType(data[i, TYPE_INDEX]);
                int[] inputs = null;
                int[] outs = null;

                var inputCount = data[i, INPUT_COUNT_INDEX];
                //2以上の時、リスト２に示される番号の信号線が入力
                if (inputCount > 1)
                {
                    inputs = new int[inputCount];
                    var start = data[i, INPUT_INDEX] - 1;
                    for (int j = 0; j < inputCount; j++)
                    {
                        inputs[j] = circleInputs.Inputs[start + j];                        
                    }
                }//1の時、3列目は入力数を示す
                else { inputs = new int[1] { data[i, INPUT_INDEX] }; }

                var outCount = data[i, OUT_COUNT_INDEX];
                //２以上の時、リスト２に示される信号線が出力
                if (outCount > 1)
                {
                    outs = new int[outCount];
                    var start = data[i, OUT_INDEX] - 1;
                    for (int j = 0; j < outCount; j++)
                    {
                        outs[j] = circleInputs.Inputs[start + j];
                    }
                }//1の時、５列目は出力数を示す
                else { outs = new int[1] { data[i, OUT_INDEX] }; }


                result[i] = new CircleData(index, type, inputs, outs);

                index++;
            }
            //優先順位決定
            DetectPriority(result);
            return result;
        }

    }
}
