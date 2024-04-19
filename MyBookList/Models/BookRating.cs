﻿using MyBookList.Enums;

namespace MyBookList.Models;

public class BookRating
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string BookId { get; set; }
    public int Rating { get; set; }
    public ReadingStatus Status { get; set; }

    public ApplicationUser User { get; set; }
    public Book Book { get; set; }
}