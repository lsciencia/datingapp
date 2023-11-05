import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getUsers();
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  getUsers() {
    this.http.get('https://localhost:7167/api/users')
      .subscribe({
        next: (response) => {
          this.users = response;
        },
        error: (err) => {
          console.log(err)
        },
        complete: () => { }
      });
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }
}
