/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Domain types                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Enumeration                          *
*  Type     : TransactionSlipStatus                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Enumerates the processing status of a transaction slip.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips {

  public enum TransactionSlipStatus {

    Pending = 'P',

    Processed = 'R',

    ProcessedOK = 'C',

    ProcessedWithIssues = 'E',

  }  // enum TransactionSlipStatus

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips

