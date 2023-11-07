/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues                                 Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : SectorsUseCases                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for sectors.                                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for sectors.</summary>
  public class SectorsUseCases : UseCase {

    #region Constructors and parsers

    protected SectorsUseCases() {
      // no-op
    }

    static public SectorsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<SectorsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<NamedEntityDto> GetSectors() {
      FixedList<Sector> sectors = Sector.GetList();

      return sectors.MapToNamedEntityList();
    }

    #endregion Use cases

  }  // class SectorsUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
