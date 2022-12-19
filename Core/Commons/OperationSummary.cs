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
using System.Collections.Generic;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds summary information about a command or operation
  /// already executed or invoked as dryrun.</summary>
  public class OperationSummary {

    private readonly List<string> _items = new List<string>(32);
    private readonly List<string> _errors = new List<string>(32);

    public string Operation {
      get; internal set;
    }

    public int Count {
      get; internal set;
    }

    public int Errors {
      get {
        return _errors.Count;
      }
    }

    public FixedList<string> ItemsList {
      get {
        return _items.ToFixedList();
      }
    }


    public FixedList<string> ErrorsList {
      get {
        return _errors.ToFixedList();
      }
    }


    internal void AddErrors(IEnumerable<string> issues) {
      _errors.AddRange(issues);
    }


    internal void AddItem(string item) {
      _items.Add(item);
    }

  }  // class OperationSummary

}  // namespace Empiria.FinancialAccounting
