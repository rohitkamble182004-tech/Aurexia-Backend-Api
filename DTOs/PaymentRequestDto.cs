using System.ComponentModel.DataAnnotations;

public class PaymentRequestDto
{
    [Required]
    public decimal Amount { get; set; }

    [Required]
    public required string Provider { get; set; }
}