// CliLib.h

#pragma once

#include <functional>

#using <system.drawing.dll>
using namespace System;
using namespace System::Drawing;
using namespace System::Runtime::InteropServices;

namespace CliLib {

	public ref class BitmapInfo
	{
	public:
		int width;
		int height;
		int byteperpixel;
		IntPtr buf;
		String ^fullname;
	};
	[StructLayout(LayoutKind::Sequential)]
	public ref class RenderParams
	{
	public:
		double left;
		double top;
		double right;
		double bottom;
		double uv_left;
		double uv_top;
		double uv_right;
		double uv_bottom;
		char red;
		char green;
		char blue;
		char alpha;
		IntPtr texture;
	};
	public ref class UIEngineCallbacks
	{
	public:
		virtual void OnDrawFill(RenderParams^ param) = 0;
		virtual void OnDrawOutline(RenderParams^ param) = 0;
		virtual void OnDrawTexture(RenderParams^ param) = 0;

		virtual IntPtr OnLoadTexture(BitmapInfo ^bitmap) = 0;
		virtual void OnReleaseTexture(IntPtr ptr) = 0;

	};

	public ref class UIEngine
	{
	public:
		static UIEngine^ mInst;
		static UIEngine^ GetInst();

		UIEngineCallbacks^ mOps;
		RenderParams^ mParam;
		void Init(UIEngineCallbacks^ ops);
		void Load(String ^fullname);

		static void DrawFill(DrawingParams param);
		static void DrawOutline(DrawingParams param);
		static void DrawTexture(DrawingParams param);
		static void* LoadTexture(jUIBitmap *bitmap);
		static void ReleaseTexture(void *ptr);

	private:
		UIEngine();
		~UIEngine();
	};
}
