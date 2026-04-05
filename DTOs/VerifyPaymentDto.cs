using System.ComponentModel.DataAnnotations;

public class VerifyPaymentDto
{
    [Required]
    public required string OrderId { get; set; }

    [Required]
    public required string PaymentId { get; set; }

    [Required]
    public required string Signature { get; set; }
}