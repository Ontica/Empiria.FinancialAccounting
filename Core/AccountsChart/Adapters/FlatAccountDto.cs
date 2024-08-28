/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : FlatAccountDto                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO to describe a flat financial accounting account.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO used to describe a flat financial accounting account.</summary>
  public class FlatAccountDto {

    internal FlatAccountDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Number {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string Description {
      get; internal set;
    }

    public NamedEntityDto Type {
      get; internal set;
    }

    public AccountRole Role {
      get; internal set;
    }

    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    }

    public int Level {
      get; internal set;
    }

    public bool LastLevel {
      get; internal set;
    }

    public Currency Currency {
      get; internal set;
    }

    public Sector Sector {
      get; internal set;
    }

    public bool SummaryWithNotChildren {
      get; internal set;
    }

  }  // class FlatAccountDto

}  // namespace Empiria.FinancialAccounting.Adapters
