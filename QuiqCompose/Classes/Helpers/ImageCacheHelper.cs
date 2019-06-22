using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;

namespace SDSK.QuiqCompose.WinDesktop.Classes.Helpers {
    public static class ImageCacheHelper {
        /// <summary>
        /// Caches a SNS account's image from URL.
        /// </summary>
        /// <param name="url">Absolute image URL to be downloaded from.</param>
        /// <param name="screenName">ScreenName of the account.</param>
        /// <param name="userId">User ID of the account.</param>
        /// <param name="cacheType">Specifies the type of image.</param>
        /// <param name="overwrite">Whether to overwrite existing cached image.</param>
        /// <returns>`true` if the cache process has done successfully, `false` if not.</returns>
        /// 
        /// <exception cref="InvalidOperationException" />
        public static bool Cache(string url, string screenName, long userId, ImageCacheType cacheType, bool overwrite = true) {
            #region 1. Download file from `url`
            string tempFilePath = Path.GetTempFileName();

            WebClient client = new WebClient();
            client.DownloadFile(url, tempFilePath);

            if(!File.Exists(tempFilePath)) {
                return false;
            }
            #endregion

            #region 2. Determine image type for downloaded file
            var header = new byte[8];
            ImageHeaderInfo.ImageType determinedType = ImageHeaderInfo.ImageType.UNKNOWN;
            
            using(var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.None)) {
                if(fileStream.Length > 0) {
                    fileStream.Read(header, 0, header.Length);
                }
            }

            var headerHexString = string.Concat(header.Select(v => v.ToString("X2")).ToArray());
            
            foreach(var type in ImageHeaderInfo.StringHexOfTypes) {
                if(headerHexString.StartsWith(type.Value)) {
                    determinedType = type.Key;
                    break;
                }
            }

            if(determinedType == ImageHeaderInfo.ImageType.UNKNOWN) {
                return false;   // If the process has failed or not supported format
            }
            #endregion

            #region 3. Move/Overwrite downloaded temp image file into QuiqCompose cache folder
            string destinationFileName = $"{screenName}_{userId}-{cacheType.ToString().ToLower()}";
            string destinationFullPath = Path.Combine(ApplicationData.Instance.GetImageCacheFolderAbsolutePath(), destinationFileName);

            if(!Directory.Exists(ApplicationData.Instance.GetImageCacheFolderAbsolutePath())) {
                Directory.CreateDirectory(ApplicationData.Instance.GetImageCacheFolderAbsolutePath());
            }

            if(overwrite && File.Exists(destinationFullPath)) {
                File.Delete(destinationFullPath);
            } else if(!overwrite && File.Exists(destinationFullPath)) {
                throw new InvalidOperationException("Cached file already exists and overwriting is not permitted.");
            }

            File.Move(tempFilePath, destinationFullPath);

            if(!File.Exists(destinationFullPath)) {
                return false;
            }
            #endregion

            #region 4. Check if cached image can be loaded into BitmapImage
            try {
                var testImage = new BitmapImage(new Uri(destinationFullPath));

                if(testImage != null && testImage.Height > 0 && testImage.Width > 0) {
                    return true;
                }
            } catch(Exception) {
                return false;
            }
            #endregion

            return false;
        }

        public static BitmapImage LoadCachedImage(string screenName, long userId, ImageCacheType cacheType) {
            string fileName = $"{screenName}_{userId}-{cacheType.ToString().ToLower()}";
            string fullPath = Path.Combine(ApplicationData.Instance.GetImageCacheFolderAbsolutePath(), fileName);

            if(File.Exists(fullPath) && (new FileInfo(fileName)).Length > 0) {
                var image = new BitmapImage(new Uri(fullPath));

                if(image != null && (image.Width > 0 && image.Height > 0)) {
                    return image;
                }
            }

            return null;
        }

        public static bool DeleteCachedImage(string screenName, string userId, ImageCacheType cacheType) {
            string fileName = $"{screenName}_{userId}-{cacheType.ToString().ToLower()}";
            string fullPath = Path.Combine(ApplicationData.Instance.GetImageCacheFolderAbsolutePath(), fileName);

            if(File.Exists(fullPath)) {
                File.Delete(fullPath);

                if(!File.Exists(fullPath)) {
                    return true;
                }
            }

            return false;
        }

        public static class ImageHeaderInfo {
            public const string JPG = "FFD8";
            public static readonly byte[] JPG_BYTE = { 0xFF, 0xD8 };
            public const string GIF = "474946";
            public static readonly byte[] GIF_BYTE = { 0x47, 0x49, 0x46 };
            public const string PNG = "89504E470D0A1A0A";
            public static readonly byte[] PNG_BYTE = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            public const string WEBP = "52494646";
            public static readonly byte[] WEBP_BYTE = { 0x52, 0x49, 0x46, 0x46 };

            public static readonly Dictionary<ImageType, string> StringHexOfTypes = new Dictionary<ImageType, string>();
            public static readonly Dictionary<ImageType, byte[]> BytesOfTypes = new Dictionary<ImageType, byte[]>();

            public enum ImageType {
                JPG,
                GIF,
                PNG,
                WEBP,
                UNKNOWN
            }

            static ImageHeaderInfo() {
                StringHexOfTypes.Add(ImageType.JPG, JPG);
                StringHexOfTypes.Add(ImageType.GIF, GIF);
                StringHexOfTypes.Add(ImageType.PNG, PNG);
                StringHexOfTypes.Add(ImageType.WEBP, WEBP);

                BytesOfTypes.Add(ImageType.JPG, JPG_BYTE);
                BytesOfTypes.Add(ImageType.GIF, GIF_BYTE);
                BytesOfTypes.Add(ImageType.PNG, PNG_BYTE);
                BytesOfTypes.Add(ImageType.WEBP, WEBP_BYTE);
            }
        }

        public enum ImageCacheType {
            PROFILE,
            BANNER
        }
    }
}
