using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFT.Attributes
{
  public  class DataRun
    {
        public ulong ClustersInRun { get; }
        public long ClusterOffset { get; }

        public DataRun(ulong clustersInRun, long clusterOffset)
        {
            ClustersInRun = clustersInRun;
            ClusterOffset = clusterOffset;
        }
    }
}
