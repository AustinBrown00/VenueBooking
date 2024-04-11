using System;
using System.Collections.Generic;

namespace Assignment1
{
    public class Venue
    {
        private string[,] seats;
        private List<string> waitlist;

        public int Rows { get; }
        public int SeatsPerRow { get; }
        public int TotalSeats => Rows * SeatsPerRow;
        public int AvailableSeats => TotalSeats - TotalBookedSeats;

        public int TotalBookedSeats
        {
            get
            {
                int bookedSeats = 0;
                foreach (var seat in seats)
                {
                    if (!string.IsNullOrEmpty(seat))
                    {
                        bookedSeats++;
                    }
                }
                return bookedSeats;
            }
        }

        public int WaitlistCount => waitlist.Count;

        public Venue(int rows, int seatsPerRow)
        {
            Rows = rows;
            SeatsPerRow = seatsPerRow;
            seats = new string[Rows, SeatsPerRow];
            waitlist = new List<string>();
        }

        public bool IsSeatBooked(int row, int seat)
        {
            return !string.IsNullOrEmpty(seats[row, seat]);
        }

        public void BookSeat(int row, int seat, string passengerName)
        {
            if (IsSeatBooked(row, seat))
            {
                throw new InvalidOperationException("Seat is already booked.");
            }
            seats[row, seat] = passengerName;
        }

        public void CancelBooking(int row, int seat)
        {
            if (!IsSeatBooked(row, seat))
            {
                throw new InvalidOperationException("Seat is not booked.");
            }
            seats[row, seat] = null;
        }

        public void AddToWaitlist(string passengerName)
        {
            waitlist.Add(passengerName);
        }

        public void CancelAllBookings()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int seat = 0; seat < SeatsPerRow; seat++)
                {
                    seats[row, seat] = null;
                }
            }
            waitlist.Clear();
        }


        public string GetBookedBy(int row, int seat)
        {
            if (!IsSeatBooked(row, seat))
            {
                throw new InvalidOperationException("Seat is not booked.");
            }

            return seats[row, seat];
        }

    }
}
