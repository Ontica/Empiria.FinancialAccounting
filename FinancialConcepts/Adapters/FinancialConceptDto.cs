/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Transfer Object                    *
*  Type     : FinancialConceptDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Financial concept data transfer object.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Financial concept data transfer object.</summary>
  public class FinancialConceptDto {

    public string UID {
      get; internal set;
    }

    public string Code {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public int Position {
      get; internal set;
    }

    public int Level {
      get; internal set;
    }

    public DateTime StartDate {
      get; internal set;
    }

    public DateTime EndDate {
      get; internal set;
    }

    public NamedEntityDto Group {
      get; internal set;
    }

    public FixedList<FinancialConceptEntryDto> Integration {
      get; internal set;
    }

  }  // class FinancialConceptDto



  /// <summary>Financial concept descriptor DTO for use in lists.</summary>
  public class FinancialConceptDescriptorDto {

    internal FinancialConceptDescriptorDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Code {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public int Position {
      get; internal set;
    }

    public int Level {
      get; internal set;
    }

    public string AccountsChartName {
      get; internal set;
    }

    public string GroupName {
      get; internal set;
    }

  }  // class FinancialConceptDescriptorDto

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
