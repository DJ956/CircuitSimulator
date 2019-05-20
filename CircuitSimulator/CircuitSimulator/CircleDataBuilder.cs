using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CircleDataBuilder
    {
        private static int TYPE_INDEX = 0;
        private static int INPUT_COUNT_INDEX = 1;
        private static int INPUT_INDEX = 2;
        private static int OUT_COUNT_INDEX = 3;
        private static int OUT_INDEX = 4;

        public CircleDataBuilder() { }

        /// <summary>
        /// txtから回路情報を読み取り、Circleリストを作成する
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<CircleData> BuildCircles(string fileName)
        {
            var data = DataIO.LoadCircleFromTxt(fileName);
            var result = new List<CircleData>(data.Count);

            var index = 1;
            foreach (var list in data)
            {
                var type = CircleTypeDict.GetType(list[TYPE_INDEX]);
                int[] inputs = null;
                int[] outs = null;

                var inputCount = list[INPUT_COUNT_INDEX];
                //2以上の時、リスト２に示される番号の信号線が入力
                if (inputCount > 1)
                {
                    inputs = new int[inputCount];
                    var start = list[INPUT_INDEX] - 1;
                    for (int i = 0; i < inputCount; i++)
                    {
                        inputs[i] = CircleInputs.Inputs[start + i];
                    }
                }//1の時、3列目は入力数を示す
                else { inputs = new int[1] { list[INPUT_INDEX] }; }

                var outCount = list[OUT_COUNT_INDEX];
                //２以上の時、リスト２に示される信号線が出力
                if (outCount > 1)
                {
                    outs = new int[outCount];
                    var start = list[OUT_INDEX] - 1;
                    for (int i = 0; i < outCount; i++)
                    {
                        outs[i] = CircleInputs.Inputs[start + i];
                    }
                }//1の時、５列目は出力数を示す
                else { outs = new int[1] { list[OUT_INDEX] }; }

                var circle = new CircleData(index, type, inputs, outs);
                result.Add(circle);

                index++;
            }
            return result;
        }
    }
}
