﻿using System.IO;

namespace Packager.Providers
{
    public interface IFileProvider
    {
        void Move(string sourceFileName, string destFileName);
    }

    public class FileProvider : IFileProvider
    {
        public void Move(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }
    }
}