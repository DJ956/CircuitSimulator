using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulator
{
    public class CirclesRawData
    {
        public List<List<int>> CircleRawlist { get; private set; }
        public CircleInputs CircleInputs { get; private set; }
        public CircleOutSideInputs CircleOutSideInputs { get; private set; }
        public CircleOutsideOutputs CirclesOutSideOutputs { get; private set; }

        /// <summary>
        /// テキストデータtblと入力パターンデータpatから読み取ったデータを一時的に格納しておくデータクラス
        /// </summary>
        /// <param name="circleRawlist">回路データ</param>
        /// <param name="inputs">リスト2</param>
        /// <param name="outsideInputs">外部入力データ</param>
        /// <param name="outsideOutputs">外部出力データ</param>
        public CirclesRawData(List<List<int>> circleRawlist, List<int> inputs, List<int> outsideInputs, List<int> outsideOutputs)
        {
            CircleRawlist = circleRawlist;
            CircleInputs = new CircleInputs(inputs);
            CircleOutSideInputs = new CircleOutSideInputs(outsideInputs);
            CirclesOutSideOutputs = new CircleOutsideOutputs(outsideOutputs);
        }

        public CirclesRawData(List<List<int>> circleRawlist, List<int> inputs, List<int> outsideInputs)
        {
            CircleRawlist = circleRawlist;
            CircleInputs = new CircleInputs(inputs);
            CircleOutSideInputs = new CircleOutSideInputs(outsideInputs);
        }

    }
}
