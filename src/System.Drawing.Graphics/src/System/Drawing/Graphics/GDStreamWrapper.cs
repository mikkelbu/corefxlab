﻿// Copyright (c) Microsoft. All rights reserved. 
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
//#define WINDOWS

using System; 
 using System.Collections.Generic; 
 using System.IO; 
 using System.Runtime.InteropServices; 
 using System.Text; 
 using System.Threading.Tasks;


namespace System.Drawing.Graphics
{
#if WINDOWS
    public class gdStreamWrapper
	{
		internal DLLImports.gdIOCtx IOCallbacks;

		Stream _stream;

		public gdStreamWrapper(Stream stream)
		{
			_stream = stream;
			IOCallbacks = new DLLImports.gdIOCtx();

			IOCallbacks.getC = getC;
			IOCallbacks.getBuf = getBuf;
			IOCallbacks.putC = putC;
			IOCallbacks.putBuf = putBuf;
			IOCallbacks.seek = seek;
			IOCallbacks.tell = tell;
		}


		int getC(IntPtr ctx)
		{
			return _stream.ReadByte();
		}

		int getBuf(IntPtr ctx, IntPtr buf, int wanted)
		{
			byte[] buffer = new byte[wanted];
			int read = _stream.Read(buffer, 0, wanted);
			if (read > 0)
			{
				Marshal.Copy(buffer, 0, buf, read);
			}
			return read;
		}

		void putC(IntPtr ctx, int ch)
		{
			_stream.WriteByte((byte)ch);
		}

		int putBuf(IntPtr ctx, System.IntPtr buf, int wanted)
		{
			byte[] buffer = new byte[wanted];
			Marshal.Copy(buf, buffer, 0, wanted);
			_stream.Write(buffer, 0, wanted);
			return wanted;
		}

		int seek(IntPtr ctx, int pos)
		{
			_stream.Seek(0, SeekOrigin.Begin);
			return 1;
		}

		int tell(IntPtr ctx)
		{
			return (int)_stream.Position;
		}
	}
}

#else

    public class gdStreamWrapper
	{
		internal LibGDLinuxImports.gdIOCtx IOCallbacks;

		Stream _stream;

		public gdStreamWrapper(Stream stream)
		{
			_stream = stream;
			IOCallbacks = new LibGDLinuxImports.gdIOCtx();

			IOCallbacks.getC = getC;
			IOCallbacks.getBuf = getBuf;
			IOCallbacks.putC = putC;
			IOCallbacks.putBuf = putBuf;
			IOCallbacks.seek = seek;
			IOCallbacks.tell = tell;
		}


		int getC(IntPtr ctx)
		{
			return _stream.ReadByte();
		}

		int getBuf(IntPtr ctx, IntPtr buf, int wanted)
		{
			byte[] buffer = new byte[wanted];
            //Console.WriteLine("\nBefore Stream Length: {0} Position: {1}", _stream.Length, _stream.Position);

            int read = _stream.Read(buffer, 0, wanted);
            //System.Console.WriteLine("Wanted:  {0} Read: {1}", wanted, read);
            //Console.WriteLine("After Stream Length: {0} Position: {1}", _stream.Length, _stream.Position);
            while (read < wanted)
            {
                //Console.WriteLine("\nInLoop Stream Length: {0} Position: {1}", _stream.Length, _stream.Position);

                int newRead = _stream.Read(buffer, read, (wanted - read));
                if (newRead == 0) break;
                read += newRead;
            }
            if (read > 0)
			{
				Marshal.Copy(buffer, 0, buf, read);
			}
			return read;
		}

		void putC(IntPtr ctx, int ch)
		{
			_stream.WriteByte((byte)ch);
		}

		int putBuf(IntPtr ctx, System.IntPtr buf, int wanted)
		{
			byte[] buffer = new byte[wanted];
			Marshal.Copy(buf, buffer, 0, wanted);
			_stream.Write(buffer, 0, wanted);
			return wanted;
		}

		int seek(IntPtr ctx, int pos)
		{
			_stream.Seek(0, SeekOrigin.Begin);
			return 1;
		}

		int tell(IntPtr ctx)
		{
			return (int)_stream.Position;
		}
	}
}

#endif