using Image_Filtering_App.Filters.Quantization.OctreeNode;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Image_Filtering_App.Filters.Quantization.OctreeDS
{
    public class Octree(int MaxLeaves)
    {
        public Node? Root { get; set; } = null;
        public int LeafCount { get; set; } = 0;
        public int MaxLeaves { get; set; } = MaxLeaves;
        public List<Node>[] InnerNodes { get; set; } = [.. Enumerable.Range(0, 8).Select(_ => new List<Node>())];

        public void Build(BitmapSource source)
        {
            WriteableBitmap image = new WriteableBitmap(source);
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int stride = image.BackBufferStride;

            image.Lock();
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)image.BackBuffer;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * stride + x * 4;

                            byte r = ptr[index + 2];
                            byte g = ptr[index + 1];
                            byte b = ptr[index];

                            Add(r, g, b);
                            while (LeafCount > MaxLeaves)
                            {
                                Reduce();
                            }
                        }
                    }
                }

                image.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                image.Unlock();
            }
        }
        private void Reduce()
        {
            int i = 7;
            for (; i >= 0; i--)
                if (InnerNodes[i].Count > 0) break;
            Node node = InnerNodes[i].Last();
            InnerNodes[i].RemoveAt(InnerNodes[i].Count - 1);
            int removed = 0;
            for (int k = 0; k < 8; k++)
            {
                if (node.children[k] != null)
                {
                    node.sumR += node.children[k].sumR;
                    node.sumG += node.children[k].sumG;
                    node.sumB += node.children[k].sumB;
                    node.count += node.children[k].count;
                    node.children[k] = null!;
                    removed++;
                }
            }
            node.isLeaf = true;
            LeafCount += 1 - removed;
        }
        private void Add(byte r, byte g, byte b)
        {
            Root ??= CreateNode(0);
            AddRecursive(Root, r, g, b, 0);
        }
        private void AddRecursive(Node parent, byte r, byte g, byte b, int depth)
        {
            if (parent.isLeaf)
            {
                parent.sumR += r;
                parent.sumG += g;
                parent.sumB += b;
                parent.count++;
            }
            else
            {
                int i = GetChildIndex(r, g, b, depth);
                Node child = parent.children[i];
                if (child == null)
                {
                    child = CreateNode(depth + 1);
                    parent.children[i] = child;
                }
                AddRecursive(child, r, g, b, depth + 1);
            }
        }
        private static int GetChildIndex(byte r, byte g, byte b, int depth)
        {
            int bitR = r >> (7 - depth) & 0x1;
            int bitG = g >> (7 - depth) & 0x1;
            int bitB = b >> (7 - depth) & 0x1;

            return (bitR << 2) | (bitG << 1) | bitB;
        }
        private Node CreateNode(int depth)
        {
            Node newNode = new(depth);
            if (newNode.isLeaf)
            {
                LeafCount++;
            }
            else
            {
                InnerNodes[depth].Add(newNode);
            }

            return newNode;
        }

        public BitmapSource ReplaceColors(BitmapSource source)
        {
            WriteableBitmap image = new WriteableBitmap(source);
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int stride = image.BackBufferStride;

            image.Lock();
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)image.BackBuffer;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = y * stride + x * 4;

                            (byte r, byte g, byte b) = Find(ptr[index + 2], ptr[index + 1], ptr[index]);

                            ptr[index + 2] = r;
                            ptr[index + 1] = g;
                            ptr[index] = b;
                        }
                    }
                }

                image.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                image.Unlock();
            }

            return image;
        }
        private (byte r, byte g, byte b) Find(byte r, byte g, byte b)
        {
            Node node = Root!;
            int depth = 0;
            while (!node.isLeaf)
            {
                int i = GetChildIndex(r, g, b, depth++);
                node = node.children[i];
            }

            return ((byte)(node.sumR / node.count), (byte)(node.sumG / node.count), (byte)(node.sumB / node.count));
        }
    }
}