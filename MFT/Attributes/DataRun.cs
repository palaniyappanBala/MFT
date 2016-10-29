namespace MFT.Attributes
{
    public class DataRun
    {
        public DataRun(ulong clustersInRun, long clusterOffset)
        {
            ClustersInRun = clustersInRun;
            ClusterOffset = clusterOffset;
        }

        public ulong ClustersInRun { get; }
        public long ClusterOffset { get; }

        public override string ToString()
        {
            return $"Cluster offset: {ClusterOffset}, # clusters: {ClustersInRun}";
        }
    }
}