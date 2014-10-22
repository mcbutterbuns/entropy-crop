<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.configuration.dll</Reference>
  <Namespace>System.Configuration</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Drawing.Drawing2D</Namespace>
</Query>

void Main()
{
	string root;

	root = @"C:\Users\Jonathan\Dropbox\Docs\";

    ScaleAndCrop(root + @"cat.png").Save(root + @"cat-out.png", ImageFormat.Jpeg);
    ScaleAndCrop(root + @"dog.png").Save(root + @"dog-out.png", ImageFormat.Png);
    ScaleAndCrop(root + @"duck.png").Save(root + @"duck-out.png", ImageFormat.Png);
    ScaleAndCrop(root + @"entropy3.jpg").Save(root + @"entropy3-out.jpg", ImageFormat.Jpeg);
}

private static Bitmap ScaleAndCrop(string file, int width = 150, int height = 150, int slice = 16){

    using (Bitmap bm = Scale(file, width, height))
    {
        return Crop(bm, width, height);
    }
}

private static Bitmap Scale(string file, int width, int height){

    double percentage;

    using (Bitmap bm = new Bitmap(file))
    {
        int originalW = bm.Width;
        int originalH = bm.Height;

		if (bm.Width < bm.Height){
			percentage = ((float)width / (float)bm.Width);
		} else{
			percentage = ((float)height / (float)bm.Height);
		}

        //get the new size based on the percentage change
        int resizedW = (int)(originalW * percentage);
        int resizedH = (int)(originalH * percentage);

        //create a new Bitmap the size of the new image
        Bitmap bmp = new Bitmap(resizedW, resizedH);

        using (Graphics graphic = Graphics.FromImage((Image)bmp)){
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(bm, 0, 0, resizedW, resizedH);
        }

        return bmp;
    }
}

private static Bitmap Crop(Bitmap image, int cropWidth, int cropHeight){

    int imgWidth, imgHeight, x = 0, y = 0, sliceWidth, sliceHeight, slice = 16;
    Bitmap left, right, top, bottom;

    imgWidth = image.Width;
    imgHeight = image.Height;

    while ((imgWidth - x) > cropWidth){

        sliceWidth = Math.Min(imgWidth - x - cropWidth, slice);

        using(left = Crop(image, x, 0, sliceWidth, imgHeight))
        using(right = Crop(image, imgWidth - sliceWidth, 0, sliceWidth, imgHeight)){

            if (Entropy(left) < Entropy(right)){
                x += sliceWidth;
            }
            else{
                imgWidth -= sliceWidth;
            }
        }
    }

    while ((imgHeight - y) > cropHeight){
        sliceHeight = Math.Min(imgHeight - y - cropHeight, slice);

        using(top = Crop(image, 0, y, imgWidth, sliceHeight))
        using(bottom = Crop(image, 0, imgHeight - sliceHeight, imgWidth, sliceHeight)){

            if (Entropy(top) < Entropy(bottom))
                y += sliceHeight;
            else
                imgHeight -= sliceHeight;
        }
    }

    return Crop(image, x, y, cropWidth, cropHeight);

}

private static Bitmap Crop(Bitmap img, int x, int y, int width, int height)
{
    Rectangle cropRect = new Rectangle(0, 0, width, height);
    Bitmap target = new Bitmap(width, height, PixelFormat.Format24bppRgb);

    using (Graphics g = Graphics.FromImage(target)){
        g.DrawImage(img, cropRect, x, y, width, height, GraphicsUnit.Pixel);
    }

    return target;
}

private static double Entropy(Bitmap bm){

    double p, area, entropy = 0;
    Dictionary<int, int> values = new Dictionary<int, int>();
    int color;
    Color current;

    area = bm.Width * bm.Height;

    //calculate the histogram
    for (int w = 0; w < bm.Width; w++){
        for(int h = 0; h < bm.Height; h++){
            current = bm.GetPixel(w,h);

            color = current.R + current.G + current.B;

            //convert to grayscale
            color = (int)((current.R * .3) + (current.G * .59) + (current.B * .11));

            if (values.ContainsKey(color) == false)
                values.Add(color, 0);

            values[color]++;
        }
    }

    foreach(KeyValuePair<int, int> c in values){

        p = (c.Value / area);
        entropy += Math.Abs(p * Math.Log(p, 2));

    }

    return entropy;
}
