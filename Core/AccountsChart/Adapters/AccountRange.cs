/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information Holder                      *
*  Type     : AccountRange                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Structure used to represent a range of financial accounts.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Structure used to represent a range of financial accounts.</summary>
  public class AccountRange {

    public string FromAccount {
      get; set;
    } = string.Empty;


    public string ToAccount {
      get; set;
    } = string.Empty;

  }  // class AccountRange

}  // namespace Empiria.FinancialAccounting.Adapters
