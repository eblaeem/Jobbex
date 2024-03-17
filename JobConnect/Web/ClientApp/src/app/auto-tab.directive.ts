import { Directive, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[appAutoTab]'
})

export class AutoTabDirective {

  @Input() libAutoTab : any;

  constructor(){ }

  @HostListener('input', ['$event.target']) onInput(input:any) {
    const length = input.value.length;
    const maxLength = input.attributes.maxlength.value;

    if (length >= maxLength && this.libAutoTab) {
      const field = document.getElementById(this.libAutoTab + 1);
      if (field) {
        field.focus();
      }
    }
  }

}
