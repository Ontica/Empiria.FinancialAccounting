/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information holder                      *
*  Type     : OperationSummary                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds summary information about a command or operation already executed or invoked as dryrun.  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds summary information about a command or operation
  /// already executed or invoked as dryrun.</summary>
  public class OperationSummary {

    public string Operation {
      get; internal set;
    }

    public int Count {
      get; internal set;
    }

    public int Errors {
      get; internal set;
    }

    public FixedList<string> ItemsList {
      get; internal set;
    } = new FixedList<string>();


    public FixedList<string> ErrorsList {
      get; internal set;
    } = new FixedList<string>();

  }  // class OperationSummary

}  // namespace Empiria.FinancialAccounting
