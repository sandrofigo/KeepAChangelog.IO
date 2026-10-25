namespace KeepAChangelog.IO.Tests;

public class CategoryTests
{
    [Fact]
    public void CompareTo_EarlierType_IsLess()
    {
        var added = new Category { Type = CategoryType.Added };
        var fixedCategory = new Category { Type = CategoryType.Fixed };

        added.CompareTo(fixedCategory).ShouldBeLessThan(0);
        fixedCategory.CompareTo(added).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void CompareTo_SameType_IsZero()
    {
        new Category { Type = CategoryType.Changed }.CompareTo(new Category { Type = CategoryType.Changed }).ShouldBe(0);
    }

    [Fact]
    public void CompareTo_Null_IsGreater()
    {
        new Category().CompareTo(null).ShouldBe(1);
    }

    [Fact]
    public void Order_ShuffledCategories_SortedByCategoryTypeOrder()
    {
        List<Category> categories =
        [
            new() { Type = CategoryType.Security },
            new() { Type = CategoryType.Added },
            new() { Type = CategoryType.Removed },
            new() { Type = CategoryType.Changed },
            new() { Type = CategoryType.Fixed },
            new() { Type = CategoryType.Deprecated }
        ];

        categories.Order().Select(category => category.Type).ShouldBe(Enum.GetValues<CategoryType>());
    }
}
