// CliLib.h

#pragma once

#include <functional>

#using <system.drawing.dll>
using namespace System;
using namespace System::Drawing;
using namespace System::Runtime::InteropServices;

namespace CliLib {

	public ref class ViewInfo
	{
	public:
		jView *view;
		String ^jsonString;
		void Update(String ^json);
		System::Drawing::Rectangle ^GetRectAbsolute();
		bool Equal(ViewInfo ^info);
		int GetID();
		int GetParentID();
	};
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
		unsigned char red;
		unsigned char green;
		unsigned char blue;
		unsigned char alpha;
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
		void SetResourcePath(String^ path);
		void Load(String ^fullname);
		void LoadJson(String ^jsonText);
		void DoMouseEvent();
		void SetMouseEvent(int mouseX, int mouseY, bool down, bool triggered);
		void Draw();
		void ChangeParent(int id, int parentID);
		void ChangeNeighbor(int id, int neighborID);
		String^ ToJsonString();
		ViewInfo^ FindTopView(int mouseX, int mouseY);
		ViewInfo^ FindView(int id);
		ViewInfo^ CreateView(int type);
		void RemoveView(int id);

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
