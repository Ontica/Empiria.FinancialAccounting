/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Command                                 *
*  Type     : EditFinancialReportCommand                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : The command used to create or update financial report items.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting {

  public interface IOrderedList {

    int Count {
      get;
    }

  }

}  // interface IOrderedList
