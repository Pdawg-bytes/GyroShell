#pragma once
interface IImmersiveShellController : IUnknown
{
	virtual HRESULT Start();
	virtual HRESULT Stop(void* unknown);
	virtual HRESULT SetCreationBehavior(void* structure);

};
interface IImmersiveShellBuilder : IUnknown
{
	virtual HRESULT CreateImmersiveShellController(IImmersiveShellController** other);
};
