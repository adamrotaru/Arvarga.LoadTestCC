using System;
using System.Collections.Generic;
using System.Text;

namespace LoadTestCC.Proto1.Iface
{
    public enum StatParamAggregationStrategy
    {
        Sum,
        Average,
        Max,
        Min
    }

    /// <summary>Describes a statistics parameter</summary>
    public class StatParamDesc
    {
        public StatParamDesc(string name, StatParamAggregationStrategy aggrStrat, double value, string unit)
        {
            _name = name;
            _aggrStrat = aggrStrat;
            _value = value;
            _unit = unit;

            if (String.IsNullOrEmpty(_name)) _name = "name";
            if (String.IsNullOrEmpty(_unit)) _unit = ".";
        }

        public string _name;
        public StatParamAggregationStrategy _aggrStrat;
        public double _value;
        public string _unit;
    }
    
    public class ClientsStat
    {
        private Dictionary<string, StatParamDesc> _params = new Dictionary<string, StatParamDesc>();

        public ClientsStat(IEnumerable<StatParamDesc> paramms)
        {
            foreach(StatParamDesc p in paramms)
            {
                _params[p._name] = p;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (StatParamDesc p in _params.Values)
            {
                sb.Append($"{p._name} {(int)p._aggrStrat} {p._value} {p._unit} ");
            }
            return sb.ToString();
        }

        public string ToNiceString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (StatParamDesc p in _params.Values)
            {
                sb.Append($"{p._name}: {(int)p._value}{p._unit} ");
            }
            return sb.ToString();
        }

        public static ClientsStat FromString(string s)
        {
            int tokenPerParam = 4;
            string[] token = s.Split(' ');
            int n = (int)(token.Length / tokenPerParam);
            ClientsStat cs = new ClientsStat(new List<StatParamDesc>());
            for (int i = 0; i < n; ++i)
            {
                string name = token[tokenPerParam*i];
                int aggrStratNum = 0;
                Int32.TryParse(token[tokenPerParam*i + 1], out aggrStratNum);
                StatParamAggregationStrategy aggrStrat = (StatParamAggregationStrategy)aggrStratNum;
                double val = 0;
                Double.TryParse(token[tokenPerParam*i + 2], out val);
                string unit = token[tokenPerParam*i + 3];

                cs._params[name] = new StatParamDesc(name, aggrStrat, val, unit);
            }
            return cs;
        }

        public static ClientsStat Aggregate(IEnumerable<ClientsStat> stats)
        {
            ClientsStat aggregate = new ClientsStat(new List<StatParamDesc>());
            int n = 0;
            foreach (ClientsStat cs in stats)
            {
                if (cs?._params != null)
                {
                    ++n;
                    foreach (StatParamDesc p in cs._params.Values)
                    {
                        if (!aggregate._params.ContainsKey(p._name))
                            aggregate._params[p._name] = new StatParamDesc(p._name, p._aggrStrat, p._value, p._unit);
                        else
                        {
                            switch (p._aggrStrat)
                            {
                                case StatParamAggregationStrategy.Sum:
                                case StatParamAggregationStrategy.Average:
                                    aggregate._params[p._name]._value += p._value;
                                    break;

                                case StatParamAggregationStrategy.Max:
                                    if (p._value > aggregate._params[p._name]._value) aggregate._params[p._name]._value = p._value;
                                    break;
                                                                    
                                case StatParamAggregationStrategy.Min:
                                    if (p._value < aggregate._params[p._name]._value) aggregate._params[p._name]._value = p._value;
                                    break;
                            }
                        }
                    }
                }
            }
            // divide those which need to be averaged
            if (n > 0)
            {
                foreach (StatParamDesc p in aggregate._params.Values)
                {
                    if (p._aggrStrat == StatParamAggregationStrategy.Average)
                        p._value = p._value / (double)n;
                }
            }
            return aggregate;
        }
    }
}