//----------------------------------------------------------------------------------------
// patterns & practices - Smart Client Software Factory - Guidance Package
//
// This file was generated by the "Add View" recipe.
//
// This interface defines the contract between the Presenter and its View, following the
// Model-View-Presenter pattern. 
//
//  
//
//
// Latest version of this Guidance Package: http://go.microsoft.com/fwlink/?LinkId=62182
//----------------------------------------------------------------------------------------
using Infosys.ATR.UIAutomation.Entities;

namespace IMSWorkBench.Scripts
{
    public interface IEdit
    {
        string Path { get; set; }
        UseCase EditUseCase { get; set; }
    }
}

