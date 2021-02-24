using System.Collections.Generic;
using System.Drawing;

namespace BatWings
{
    /*
     * This class includes the weight maps of multiple bones
     */
    class WeightMap
    {
        public Dictionary<string, double>[,] Weights;
        public int Width, Height;

        HashSet<Bone> Bones;

        /**
         * Constructs a WeightMap
         * width and height specify the pixel size / resolution. 100 should be good.
         */
        public WeightMap(int width, int height)
        {
            Width = width;
            Height = height;

            Weights = new Dictionary<string, double>[Width, Height];
            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    Weights[i, j] = new Dictionary<string, double>();
                }
            }
        }

        /*
         * Calculate weights for all bones
         * Weights depend on distance and a falloff
         */
        public void CalculateDistances(HashSet<Bone> bones)
        {
            Bones = bones;

            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    // Calc weights
                    double sum = 0.0;
                    foreach(Bone bone in bones)
                    {
                        double dist = bone.Dist(new Vector((double)(1.0 / Width) * i, (double)(1.0 / Height) * j));
                        
                        // There might be better falloff functions, feel free to experiment
                        double falloff = 1.0 / dist;

                        Weights[i, j][bone.Name] = falloff;
                        sum += dist;
                    }
                }
            }
            Normalize();
        }

        /*
         * This removes all bone weighs except for the <cnt> strongest. For each pixel.
         * Normalizes each pixel afterwards.
         * 
         * This will cause hard edges, smooth with SmoothEdge() or an image processing program.
         */
        public void LimitWeightCount(int cnt)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    while (Weights[i, j].Count > cnt)
                    {
                        double worst = 1;
                        string worstStr = "";
                        foreach (var w in Weights[i, j])
                        {
                            if (w.Value < worst)
                            {
                                worst = w.Value;
                                worstStr = w.Key;
                            }
                        }
                        if (worstStr != "") Weights[i, j].Remove(worstStr);
                    }
                }
            }
            Normalize();
        }

        /*
         * Does a smooth fade to 0 weight along the specified line.
         * Modifies only the map for the specified boneName
         * Strength modifies the falloff
         * 
         * Call Normalize() after all edges have been smoothed.
         */
        public void SmoothEdge(string boneName, Bone line, double strength)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {

                    if(Weights[i, j].ContainsKey(boneName))
                    {
                        double dist = line.Dist(new Vector((double)i / Width, (double)j / Height));

                        // There might be better falloff functions, feel free to experiment. Should be 0 at distance 0
                        double falloff = 1.0 - 1.0 / (1.0 + dist * strength);

                        if (falloff > 1.0) falloff = 1.0;
                        else if (falloff < 0.0) falloff = 0.0;

                        Weights[i, j][boneName] *= falloff;
                    }

                }
            }
        }

        /**
         * Normalizes each pixel to a total weight of 1
         */
        public void Normalize()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    double sum = 0.0;
                    foreach (Bone bone in Bones)
                    {
                        if (Weights[i, j].TryGetValue(bone.Name, out double dweight)) {
                            sum += dweight;
                        }
                    }
                    // Normalize
                    if (sum > 0)
                    {
                        foreach (Bone bone in Bones)
                        {
                            if(Weights[i,j].ContainsKey(bone.Name))
                                Weights[i, j][bone.Name] /= sum;
                        }
                    }
                }
            }
        }

        /**
         * Multiplies a specific bone of a different WeightMap onto all boneWeights of this one.
         * Use this for more exact fading
         */
        public void Multiply(WeightMap map, string bone)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    double fac = 0.0;
                    if (map.Weights[i, j].ContainsKey(bone))
                    {
                        fac = map.Weights[i, j][bone];
                    }

                    foreach (Bone b in Bones) {
                        if (Weights[i, j].ContainsKey(b.Name))
                        {
                            Weights[i, j][b.Name] *= fac;
                        }
                    }
                }
            }
        }

        /**
         * Export all the weight maps of all bones to PNG pictures.
         * Import these pictures to vertex weights in Blender
         */
        public void ExportToImage(string fileName)
        {
            foreach (Bone bone in Bones) {
                using (Bitmap bm = new Bitmap(Width, Height))
                {
                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Height; j++)
                        {
                            int weight = 0;
                            if (Weights[i, j].TryGetValue(bone.Name, out double dweight))
                            {
                                weight = (int)(255.0 * dweight);
                                if (weight < 0) weight = 0;
                                else if (weight > 255) weight = 255;
                            }
                            bm.SetPixel(i, j, Color.FromArgb(weight, weight, weight));
                        }
                    }
                    bm.Save(string.Format("{0}_{1}.png", fileName, bone.Name), System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }
    }
}
