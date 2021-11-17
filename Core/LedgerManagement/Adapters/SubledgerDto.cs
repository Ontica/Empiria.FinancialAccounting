/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : SubledgerDto                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTOs related to subledger books and subleger accounts.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO for subledger books.</summary>
  public class SubledgerDto {

    internal SubledgerDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string TypeName {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string Description {
      get; internal set;
    }


    public string AccountsPrefix {
      get; internal set;
    }

  }  // public class SubledgerDto


  /// <summary>Minimal output DTO for a subledger account.</summary>
  public class SubledgerAccountDescriptorDto {

    internal SubledgerAccountDescriptorDto() {
      // no-op
    }

    public int Id {
      get; internal set;
    }

    public string TypeName {
      get; internal set;
    }

    public string LedgerName {
      get; internal set;
    }

    public string Number {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string FullName {
      get; internal set;
    }

  }  // class SubledgerAccountDescriptorDto


  /// <summary>Complete output DTO for a subledger account.</summary>
  public class SubledgerAccountDto {

    internal SubledgerAccountDto() {
      // no-op
    }

    public int Id {
      get; internal set;
    }

    public string AccountsChartUID {
      get; internal set;
    }

    public NamedEntityDto Ledger {
      get; internal set;
    }

    public NamedEntityDto Type {
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


    public FixedList<NamedEntityDto> Lists {
      get; internal set;
    }

  }  // class SubledgerAccountDto

}  // namespace Empiria.FinancialAccounting.Adapters
