// 기본 DLL 파일입니다.

#include "stdafx.h"
#include <string>

#include <msclr\marshal_cppstd.h>

#pragma comment(lib,"jGUI.lib")

#include "../../MyUIEngine/jGUI/jUISystem.h"
#include "../../MyUIEngine/jGUI/jView.h"

#include "CliLib.h"
using namespace CliLib;
using namespace msclr::interop;


UIEngine::UIEngine()
{
}

UIEngine::~UIEngine()
{
}
UIEngine^ UIEngine::GetInst()
{
	if (mInst == nullptr)
		mInst = gcnew UIEngine();
	return mInst;
}
void UIEngine::Init(UIEngineCallbacks^ ops)
{
	mParam = gcnew RenderParams();
	mOps = ops;
	jUISystem *system = jUISystem::GetInst();
	system->EventDrawFill = UIEngine::DrawFill;
	system->EventDrawOutline = UIEngine::DrawOutline;
	system->EventDrawTexture = UIEngine::DrawTexture;
	system->OpLoadTexture = UIEngine::LoadTexture;
	system->OpReleaseTexture = UIEngine::ReleaseTexture;
}
void UIEngine::Load(String ^fullname)
{
	std::string name = marshal_as<std::string>(fullname);

	jUISystem *system = jUISystem::GetInst();
	system->ParseJson(name);
	system->LoadViews();
}
void UIEngine::DrawFill(DrawingParams param)
{
	Marshal::PtrToStructure(IntPtr(&param), mInst->mParam);
	mInst->mOps->OnDrawFill(mInst->mParam);
}
void UIEngine::DrawOutline(DrawingParams param)
{
	Marshal::PtrToStructure(IntPtr(&param), mInst->mParam);
	mInst->mOps->OnDrawOutline(mInst->mParam);
}
void UIEngine::DrawTexture(DrawingParams param)
{
	Marshal::PtrToStructure(IntPtr(&param), mInst->mParam);
	mInst->mOps->OnDrawTexture(mInst->mParam);
}
void* UIEngine::LoadTexture(jUIBitmap *bitmap)
{
	BitmapInfo ^info = gcnew BitmapInfo();
	info->width = bitmap->width;
	info->height = bitmap->height;
	info->byteperpixel = bitmap->byteperpixel;
	info->buf = bitmap->buf.empty() ? IntPtr(nullptr) : IntPtr(&bitmap->buf[0]);
	info->fullname = marshal_as<String^>(bitmap->fullname);
	void* ptr = (void*)mInst->mOps->OnLoadTexture(info);
	return ptr;
}
void UIEngine::ReleaseTexture(void *ptr)
{
	mInst->mOps->OnReleaseTexture(IntPtr(ptr));
}