using System;
using System.IO;
using System.Threading.Tasks;

namespace HowlOut
{
	public interface ImageResizer
	{
		byte[] ResizeImage(byte[] imageData, float size);
	}
}