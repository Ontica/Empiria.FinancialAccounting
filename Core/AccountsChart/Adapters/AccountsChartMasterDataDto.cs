/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : AccountsChartMasterDataDto                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO that contains an accounts chart master data.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>DTO that contains an accounts chart master data.</summary>
  public class AccountsChartMasterDataDto {

    internal AccountsChartMasterDataDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string AccountsPattern {
      get; internal set;
    }


    public DateTime StartDate {
      get; internal set;
    }


    public DateTime EndDate {
      get; internal set;
    }


    public FixedList<AccountRole> AccountRoles {
      get; internal set;
    }


    public FixedList<NamedEntityDto> AccountTypes {
      get; internal set;
    }


  }  // class AccountsChartMasterDataDto

}  // namespace Empiria.FinancialAccounting.Adapters
