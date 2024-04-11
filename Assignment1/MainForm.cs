/*
 * Austin B 
 * 
 * Assignment 1
 * 
 * Feb 11 2024
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Assignment1
{
    public partial class MainForm : Form
    {
        private Venue venue;
        private ToolTip toolTip;
        private Button selectedSeatButton;

        public MainForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                venue = new Venue(3, 4);
                InitializeSeatingGrid();
                UpdateStatusLabel();
                toolTip = new ToolTip();


                lstRows.Items.AddRange(new object[] { "A", "B", "C" });

                // Populate the lstSeats list box with seat values (1, 2, 3, 4)
                lstSeats.Items.AddRange(new object[] { "1", "2", "3", "4" });

                // Ensure MainForm is visible and centered on the screen
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while initializing the MainForm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void InitializeSeatingGrid()
        {
            for (int row = 1; row <= venue.Rows; row++)
            {
                for (int seat = 1; seat <= venue.SeatsPerRow; seat++)
                {
                    string buttonName = $"btnSeat_{row}_{seat}";
                    Control[] foundControls = Controls.Find(buttonName, true);

                    if (foundControls.Length > 0 && foundControls[0] is Button btnSeat)
                    {
                        btnSeat.Click += BtnSeat_Click;
                        btnSeat.BackColor = venue.IsSeatBooked(row - 1, seat - 1) ? System.Drawing.Color.Red : System.Drawing.Color.Green;

                        // Add MouseHover event handler for each seat button
                        btnSeat.MouseHover += BtnSeat_MouseHover;
                    }
                    else
                    {
                        MessageBox.Show($"Button '{buttonName}' not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void UpdateStatusLabel()
        {
            lblStatus.Text = $"Total capacity: {venue.TotalSeats}, Available seats: {venue.AvailableSeats}, Waitlist: {venue.WaitlistCount}";
        }
        private void BtnSeat_Click(object sender, EventArgs e)
        {
            selectedSeatButton = sender as Button;
            string[] seatInfo = selectedSeatButton.Name.Split('_');
            int row = int.Parse(seatInfo[1]);
            int seat = int.Parse(seatInfo[2]);

            // Convert row number to corresponding letter (A = 1, B = 2, C = 3)
            char selectedRow = (char)('A' + row - 1);

            // Update the selected row and seat in the list boxes
            if (lstRows.Items.Count >= row && lstSeats.Items.Count >= seat)
            {
                lstRows.SelectedIndex = row - 1;
                lstSeats.SelectedIndex = seat - 1;
            }

            lblMessage.Text = venue.IsSeatBooked(row - 1, seat - 1) ? "Seat is already taken." : "Enter customer name and click Book.";
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            if (selectedSeatButton == null)
            {
                lblMessage.Text = "Please select a seat.";
                return;
            }

            string passengerName = txtPassengerName.Text.Trim();
            if (string.IsNullOrWhiteSpace(passengerName))
            {
                lblMessage.Text = "Please enter customer name.";
                return;
            }

            string[] seatInfo = selectedSeatButton.Name.Split('_');
            int row = int.Parse(seatInfo[1]);
            int seat = int.Parse(seatInfo[2]);


            Console.WriteLine($"Selected seat: {row}, {seat}");
            Console.WriteLine($"customer name: {passengerName}");

            if (venue.IsSeatBooked(row - 1, seat - 1))
            {
                lblMessage.Text = "Seat is already taken.";
                return;
            }


            try
            {
                venue.BookSeat(row - 1, seat - 1, passengerName);
                selectedSeatButton.BackColor = System.Drawing.Color.Red;
                UpdateStatusLabel();
                lblMessage.Text = $"Seat booked for {passengerName}.";
                selectedSeatButton = null;
            }
            catch (Exception ex)
            {
                // Display any exception that occurs during the booking process
                lblMessage.Text = $"An error occurred while booking the seat: {ex.Message}";
            }
        }

        

        private void btnWaitlist_Click(object sender, EventArgs e)
        {
            string passengerName = txtPassengerName.Text.Trim();
            if (string.IsNullOrWhiteSpace(passengerName))
            {
                lblMessage.Text = "Please enter customer name.";
                return;
            }

            if (venue.AvailableSeats > 0)
            {
                lblMessage.Text = "Seats are available. Cannot add to waitlist.";
                return;
            }

            venue.AddToWaitlist(passengerName);
            UpdateStatusLabel();
            lblMessage.Text = $"{passengerName} added to waitlist.";
        }

        private void btnFillAll_Click(object sender, EventArgs e)
        {
            string passengerName = txtPassengerName.Text.Trim();
            if (string.IsNullOrWhiteSpace(passengerName))
            {
                lblMessage.Text = "Please enter customer name.";
                return;
            }

            for (int row = 1; row <= venue.Rows; row++)
            {
                for (int seat = 1; seat <= venue.SeatsPerRow; seat++)
                {
                    if (!venue.IsSeatBooked(row - 1, seat - 1))
                    {
                        venue.BookSeat(row - 1, seat - 1, passengerName);
                        Button seatButton = Controls.Find($"btnSeat_{row}_{seat}", true).FirstOrDefault() as Button;
                        if (seatButton != null)
                        {
                            seatButton.BackColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
            UpdateStatusLabel();
            lblMessage.Text = $"All available seats filled with {passengerName}.";
        }

        private void btnCancelAll_Click(object sender, EventArgs e)
        {
            venue.CancelAllBookings();

            // Reset the background color of all seat buttons to green
            for (int row = 1; row <= venue.Rows; row++)
            {
                for (int seat = 1; seat <= venue.SeatsPerRow; seat++)
                {
                    string buttonName = $"btnSeat_{row}_{seat}";
                    Button seatButton = Controls.Find(buttonName, true).FirstOrDefault() as Button;
                    if (seatButton != null)
                    {
                        seatButton.BackColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        MessageBox.Show($"Button '{buttonName}' not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            UpdateStatusLabel();
            lblMessage.Text = "All bookings canceled.";
        }
        private void BtnSeat_MouseHover(object sender, EventArgs e)
        {
            Button seatButton = sender as Button;
            string[] seatInfo = seatButton.Name.Split('_');
            int row = int.Parse(seatInfo[1]);
            int seat = int.Parse(seatInfo[2]);

            if (venue.IsSeatBooked(row - 1, seat - 1))
            {
                string bookedBy = (string)venue.GetBookedBy(row - 1, seat - 1); // Assuming there's a method in the Venue class to get the booked passenger's name
                toolTip.SetToolTip(seatButton, $"Booked by Customer: {bookedBy}");
            }
            else
            {
                toolTip.SetToolTip(seatButton, "Available");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (selectedSeatButton == null)
            {
                lblMessage.Text = "Please select a seat to cancel.";
                return;
            }

            string[] seatInfo = selectedSeatButton.Name.Split('_');
            int row = int.Parse(seatInfo[1]);
            int seat = int.Parse(seatInfo[2]);

            if (!venue.IsSeatBooked(row - 1, seat - 1))
            {
                lblMessage.Text = "Seat is not booked.";
                return;
            }

            venue.CancelBooking(row - 1, seat - 1);
            selectedSeatButton.BackColor = System.Drawing.Color.Green;
            UpdateStatusLabel();
            lblMessage.Text = $"Booking canceled for seat {row}, {seat}.";


        }

        
    }
}
