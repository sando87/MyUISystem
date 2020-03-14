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
void UIEngine::SetResourcePath(String^ path)
{
	std::string _path = marshal_as<std::string>(path);
	jUISystem::GetInst()->SetResourcePath(_path);
}
void UIEngine::Load(String ^fullname)
{
	std::string name = marshal_as<std::string>(fullname);

	jUISystem *system = jUISystem::GetInst();
	system->ParseJson(name);
	system->LoadViews();
}
void UIEngine::LoadJson(String ^jsonText)
{
	std::string text = marshal_as<std::string>(jsonText);

	jUISystem *system = jUISystem::GetInst();
	system->ParseJsonString(text);
	system->LoadViews();
}
void UIEngine::DoMouseEvent()
{
	jUISystem::GetInst()->MouseEventCall();
}
void UIEngine::SetMouseEvent(int mouseX, int mouseY, bool down, bool triggered)
{
	jUISystem::GetInst()->SetMouseEvent(Point2(mouseX, mouseY), down, triggered);
}
void UIEngine::Draw()
{
	jUISystem::GetInst()->Draw();
}
void CliLib::UIEngine::ChangeParent(int id, int parentID)
{
	jUISystem::GetInst()->ChangeParent(id, parentID);
}
void CliLib::UIEngine::ChangeNeighbor(int id, int neighborID)
{
	jUISystem::GetInst()->ChangeNeighbor(id, neighborID);
}
String^ UIEngine::ToJsonString()
{
	string ret = jUISystem::GetInst()->ToJsonString();
	String^ str = marshal_as<String^>(ret);
	return str;
}
ViewInfo^ UIEngine::FindTopView(int mouseX, int mouseY)
{
	jView *topView = jUISystem::GetInst()->FindTopView(mouseX, mouseY);
	if (topView == nullptr)
		return nullptr;

	ViewInfo^ view = gcnew ViewInfo();
	view->view = topView;
	string str = topView->ToJsonString();
	view->jsonString = marshal_as<String^>(str);
	return view;
}
ViewInfo^ UIEngine::FindView(int id)
{
	jView *findView = jUISystem::GetInst()->FindView(id);
	if (findView == nullptr)
		return nullptr;

	ViewInfo^ view = gcnew ViewInfo();
	view->view = findView;
	string str = findView->ToJsonString();
	view->jsonString = marshal_as<String^>(str);
	return view;
}
ViewInfo^ UIEngine::CreateView(int type)
{
	ViewInfo^ view = gcnew ViewInfo();
	jView *newView = jUISystem::GetInst()->CreateView((jViewType)type);
	if (newView != nullptr)
	{
		string str = newView->ToJsonString();
		view->view = newView;
		view->jsonString = marshal_as<String^>(str);
	}
	return view;
}
void UIEngine::RemoveView(int id)
{
	jUISystem::GetInst()->DeleteView(id);
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
void ViewInfo::Update(String ^json)
{
	jsonString = json;
	string str = marshal_as<string>(json);
	jUISystem::GetInst()->UpdateView(view, str);
}
System::Drawing::Rectangle ^ViewInfo::GetRectAbsolute()
{
	jRectangle rect = view->GetRectAbsolute();
	return gcnew System::Drawing::Rectangle(rect.Left(), rect.Top(), rect.Width(), rect.Height());
}
bool ViewInfo::Equal(ViewInfo ^info)
{
	if (info == nullptr)
		return false;
	return info->view == view;
}
int ViewInfo::GetID()
{
	return view->GetID();
}
int ViewInfo::GetParentID()
{
	jView* parent = view->GetParent();
	return parent == nullptr ? -1 : parent->GetID();
}
