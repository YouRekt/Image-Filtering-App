namespace Image_Filtering_App.Filters.Quantization.OctreeNode
{
    public class Node
    {
        public bool isLeaf;
        public int sumR;
        public int sumG;
        public int sumB;
        public int count;
        public Node[] children = new Node[8];

        public Node(int depth)
        {
            isLeaf = depth == 8;
        }
    }
}
