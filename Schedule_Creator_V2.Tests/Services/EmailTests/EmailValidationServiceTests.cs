using Schedule_Creator_V2.Services.Email;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Schedule_Creator_V2.Tests.Services.Email
{
    public sealed class EmailValidationServiceTests
    {
        // =========================================================
        // ATTACHED PROPERTY
        // =========================================================

        [Fact]
        public void GetIsRequired_DefaultValue_ReturnsTrue()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox textBox =
                        new TextBox();


                    // Act

                    bool result =
                        EmailValidationService
                            .GetIsRequired(
                                textBox);


                    // Assert

                    Assert.True(
                        result);
                });
        }


        [Fact]
        public void SetIsRequired_False_ReturnsFalse()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox textBox =
                        new TextBox();


                    // Act

                    EmailValidationService
                        .SetIsRequired(
                            textBox,
                            false);


                    // Assert

                    Assert.False(
                        EmailValidationService
                            .GetIsRequired(
                                textBox));
                });
        }


        [Fact]
        public void SetIsRequired_True_ReturnsTrue()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox textBox =
                        new TextBox();

                    EmailValidationService
                        .SetIsRequired(
                            textBox,
                            false);


                    // Act

                    EmailValidationService
                        .SetIsRequired(
                            textBox,
                            true);


                    // Assert

                    Assert.True(
                        EmailValidationService
                            .GetIsRequired(
                                textBox));
                });
        }


        // =========================================================
        // VALID FIELDS
        // =========================================================

        [Fact]
        public void ValidateRequiredFields_AllFieldsValid_ReturnsTrue()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        new TextBox
                        {
                            Text =
                                "Subject"
                        });

                    container.Children.Add(
                        new TextBox
                        {
                            Text =
                                "Header"
                        });

                    container.Children.Add(
                        new TextBox
                        {
                            Text =
                                "Body"
                        });


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out string errorMessage);


                    // Assert

                    Assert.True(
                        result);

                    Assert.Equal(
                        string.Empty,
                        errorMessage);
                });
        }


        [Fact]
        public void ValidateRequiredFields_NoTextBoxes_ReturnsTrue()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    StackPanel container =
                        new StackPanel();


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out string errorMessage);


                    // Assert

                    Assert.True(
                        result);

                    Assert.Equal(
                        string.Empty,
                        errorMessage);
                });
        }


        // =========================================================
        // EMPTY REQUIRED FIELD
        // =========================================================

        [Fact]
        public void ValidateRequiredFields_EmptyRequiredField_ReturnsFalse()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        new TextBox
                        {
                            Text =
                                string.Empty
                        });


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out string errorMessage);


                    // Assert

                    Assert.False(
                        result);

                    Assert.False(
                        string.IsNullOrWhiteSpace(
                            errorMessage));
                });
        }


        [Fact]
        public void ValidateRequiredFields_WhitespaceRequiredField_ReturnsFalse()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        new TextBox
                        {
                            Text =
                                "     "
                        });


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out _);


                    // Assert

                    Assert.False(
                        result);
                });
        }


        // =========================================================
        // ERROR MESSAGE
        // =========================================================

        [Fact]
        public void ValidateRequiredFields_InvalidField_ReturnsExpectedErrorMessage()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        new TextBox
                        {
                            Text =
                                string.Empty
                        });


                    // Act

                    EmailValidationService
                        .ValidateRequiredFields(
                            container,
                            out string errorMessage);


                    // Assert

                    Assert.Equal(
                        "Please complete all required email fields " +
                        "before generating the email.",
                        errorMessage);
                });
        }


        // =========================================================
        // ERROR BORDER
        // =========================================================

        [Fact]
        public void ValidateRequiredFields_EmptyRequiredField_AppliesErrorBorder()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox textBox =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        textBox);


                    // Act

                    EmailValidationService
                        .ValidateRequiredFields(
                            container,
                            out _);


                    // Assert

                    SolidColorBrush brush =
                        Assert.IsType<SolidColorBrush>(
                            textBox.BorderBrush);

                    Assert.Equal(
                        Color.FromRgb(
                            220,
                            38,
                            38),
                        brush.Color);

                    Assert.Equal(
                        new Thickness(2),
                        textBox.BorderThickness);
                });
        }


        // =========================================================
        // DEFAULT BORDER
        // =========================================================

        [Fact]
        public void ValidateRequiredFields_ValidField_UsesDefaultBorder()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox textBox =
                        new TextBox
                        {
                            Text =
                                "Valid"
                        };

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        textBox);


                    // Act

                    EmailValidationService
                        .ValidateRequiredFields(
                            container,
                            out _);


                    // Assert

                    SolidColorBrush brush =
                        Assert.IsType<SolidColorBrush>(
                            textBox.BorderBrush);

                    Assert.Equal(
                        Color.FromRgb(
                            171,
                            173,
                            179),
                        brush.Color);

                    Assert.Equal(
                        new Thickness(1),
                        textBox.BorderThickness);
                });
        }


        [Fact]
        public void ValidateRequiredFields_PreviouslyInvalidFieldBecomesValid_ResetsBorder()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox textBox =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        textBox);

                    EmailValidationService
                        .ValidateRequiredFields(
                            container,
                            out _);

                    SolidColorBrush errorBrush =
                        Assert.IsType<SolidColorBrush>(
                            textBox.BorderBrush);

                    Assert.Equal(
                        Color.FromRgb(
                            220,
                            38,
                            38),
                        errorBrush.Color);


                    // Act

                    textBox.Text =
                        "Now Valid";

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out _);


                    // Assert

                    Assert.True(
                        result);

                    SolidColorBrush resetBrush =
                        Assert.IsType<SolidColorBrush>(
                            textBox.BorderBrush);

                    Assert.Equal(
                        Color.FromRgb(
                            171,
                            173,
                            179),
                        resetBrush.Color);

                    Assert.Equal(
                        new Thickness(1),
                        textBox.BorderThickness);
                });
        }


        // =========================================================
        // OPTIONAL FIELDS
        // =========================================================

        [Fact]
        public void ValidateRequiredFields_EmptyOptionalField_ReturnsTrue()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox textBox =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    EmailValidationService
                        .SetIsRequired(
                            textBox,
                            false);

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        textBox);


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out string errorMessage);


                    // Assert

                    Assert.True(
                        result);

                    Assert.Equal(
                        string.Empty,
                        errorMessage);
                });
        }


        [Fact]
        public void ValidateRequiredFields_EmptyOptionalField_DoesNotApplyErrorBorder()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox textBox =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    EmailValidationService
                        .SetIsRequired(
                            textBox,
                            false);

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        textBox);


                    // Act

                    EmailValidationService
                        .ValidateRequiredFields(
                            container,
                            out _);


                    // Assert

                    SolidColorBrush brush =
                        Assert.IsType<SolidColorBrush>(
                            textBox.BorderBrush);

                    Assert.Equal(
                        Color.FromRgb(
                            171,
                            173,
                            179),
                        brush.Color);

                    Assert.Equal(
                        new Thickness(1),
                        textBox.BorderThickness);
                });
        }


        [Fact]
        public void ValidateRequiredFields_OptionalEmptyAndRequiredValid_ReturnsTrue()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox required =
                        new TextBox
                        {
                            Text =
                                "Required Value"
                        };

                    TextBox optional =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    EmailValidationService
                        .SetIsRequired(
                            optional,
                            false);

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        required);

                    container.Children.Add(
                        optional);


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out _);


                    // Assert

                    Assert.True(
                        result);
                });
        }


        // =========================================================
        // MULTIPLE INVALID FIELDS
        // =========================================================

        [Fact]
        public void ValidateRequiredFields_MultipleInvalidFields_MarksAllInvalid()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox first =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    TextBox second =
                        new TextBox
                        {
                            Text =
                                "   "
                        };

                    TextBox third =
                        new TextBox
                        {
                            Text =
                                "Valid"
                        };

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        first);

                    container.Children.Add(
                        second);

                    container.Children.Add(
                        third);


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out _);


                    // Assert

                    Assert.False(
                        result);

                    AssertErrorBorder(
                        first);

                    AssertErrorBorder(
                        second);

                    AssertDefaultBorder(
                        third);
                });
        }


        // =========================================================
        // NESTED VISUAL TREE
        // =========================================================

        [Fact]
        public void ValidateRequiredFields_NestedTextBox_IsValidated()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox nestedTextBox =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    Grid grid =
                        new Grid();

                    grid.Children.Add(
                        nestedTextBox);

                    Border border =
                        new Border
                        {
                            Child =
                                grid
                        };

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        border);


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out _);


                    // Assert

                    Assert.False(
                        result);

                    AssertErrorBorder(
                        nestedTextBox);
                });
        }


        [Fact]
        public void ValidateRequiredFields_MultipleNestedLevels_FindsTextBox()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox textBox =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    StackPanel innerPanel =
                        new StackPanel();

                    innerPanel.Children.Add(
                        textBox);

                    Grid grid =
                        new Grid();

                    grid.Children.Add(
                        innerPanel);

                    Border border =
                        new Border
                        {
                            Child =
                                grid
                        };

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        border);


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out _);


                    // Assert

                    Assert.False(
                        result);

                    AssertErrorBorder(
                        textBox);
                });
        }


        // =========================================================
        // MIXED REQUIRED / OPTIONAL
        // =========================================================

        [Fact]
        public void ValidateRequiredFields_RequiredEmptyAndOptionalEmpty_ReturnsFalseOnlyBecauseRequired()
        {
            RunOnSta(
                () =>
                {
                    // Arrange

                    TextBox required =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    TextBox optional =
                        new TextBox
                        {
                            Text =
                                string.Empty
                        };

                    EmailValidationService
                        .SetIsRequired(
                            optional,
                            false);

                    StackPanel container =
                        new StackPanel();

                    container.Children.Add(
                        required);

                    container.Children.Add(
                        optional);


                    // Act

                    bool result =
                        EmailValidationService
                            .ValidateRequiredFields(
                                container,
                                out _);


                    // Assert

                    Assert.False(
                        result);

                    AssertErrorBorder(
                        required);

                    AssertDefaultBorder(
                        optional);
                });
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static void AssertErrorBorder(
            TextBox textBox)
        {
            SolidColorBrush brush =
                Assert.IsType<SolidColorBrush>(
                    textBox.BorderBrush);

            Assert.Equal(
                Color.FromRgb(
                    220,
                    38,
                    38),
                brush.Color);

            Assert.Equal(
                new Thickness(2),
                textBox.BorderThickness);
        }


        private static void AssertDefaultBorder(
            TextBox textBox)
        {
            SolidColorBrush brush =
                Assert.IsType<SolidColorBrush>(
                    textBox.BorderBrush);

            Assert.Equal(
                Color.FromRgb(
                    171,
                    173,
                    179),
                brush.Color);

            Assert.Equal(
                new Thickness(1),
                textBox.BorderThickness);
        }


        // =========================================================
        // STA TEST RUNNER
        // =========================================================

        private static void RunOnSta(
            Action action)
        {
            Exception? capturedException =
                null;

            Thread thread =
                new Thread(
                    () =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception exception)
                        {
                            capturedException =
                                exception;
                        }
                    });

            thread.SetApartmentState(
                ApartmentState.STA);

            thread.Start();

            thread.Join();

            if (capturedException is not null)
            {
                ExceptionDispatchInfo
                    .Capture(
                        capturedException)
                    .Throw();
            }
        }
    }
}