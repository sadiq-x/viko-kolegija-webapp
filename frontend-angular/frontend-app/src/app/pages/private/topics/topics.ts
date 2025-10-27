import { CommonModule } from '@angular/common';
import { Component, computed, EventEmitter, signal, TrackByFunction } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModelTopicsResponse } from '../../../models/modelTopics';
import { TopicsService } from '../../../services/topics';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-topics',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './topics.html',
  styleUrls: ['./topics.scss'],
})
export class Topics {
  //Array list get from backend
  topics: ModelTopicsResponse[] = [];

  //Loading status
  loading = false;

  /** (opcional) Evento quando o utilizador escolhe um tópico */
  selectTopic = new EventEmitter<ModelTopicsResponse>();

  //Search variable - signal
  query = signal<string>('');

  // Skeletons for loading array, it's a blur loading items, just used only for Ui 
  loadingSkeletons = Array.from({ length: 6 });

  //Filtered List (Type + Description)
  filtered = computed<ModelTopicsResponse[]>(() => {
    const norm = (s: string) =>
      s.normalize('NFD').replace(/\p{Diacritic}/gu, '').toLowerCase(); //Remove the accentuation and use the lowerCase character
    const q = norm(this.query().trim()); //Get the string/character search, and remove empty spaces in the start/end
    if (!q) return this.topics ?? []; //If the variable q don't have any character, return full list of Topics
    return (this.topics ?? []).filter(
      //Filter the list Topics
      (t) => norm(t.Type ?? '').includes(q) || norm(t.Description ?? '').includes(q) //Else if have character, search in Type or Description the specific string
    );
  });
  router: any;

  constructor(private topicService: TopicsService) {}

  ngOnInit() {
    this.loadTopics();
  }

  loadTopics() {
    this.loading = true; //Turn on before the request
    this.topicService.getTopics().subscribe({
      next: (res: ModelTopicsResponse[] | false) => {
        //Verify if the res are false or not
        if (res == false || !Array.isArray(res)) {
          this.topics = []; //Set the loading false
          this.loading = false;
          return;
        }

        this.loading = false; //Set the loading false
        const list = Array.isArray(res) ? res : [];
        //Mapped for the interface with PascalCase
        this.topics = list.map((x: any) => ({
          Id: x.Id ?? x.id,
          Type: x.Type ?? x.type,
          Description: x.Description ?? x.description,
        }));
      },
      error: () => {
        this.topics = [];
        this.loading = false;
      },
    });
  }
  //Function trackBy, help in the DOM rendering
  trackById: TrackByFunction<ModelTopicsResponse> = (_: number, item) => item.Id;
  //Function onSelect the Topic
  onSelect(t: ModelTopicsResponse) {
     if (!t?.Type) return;
    // navegação para /courses/type/:type (o Angular codifica espaços/acentos automaticamente)
    this.router.navigate([`/courses/typet/:${t.Type}`]);
    // Se preferires garantir codificação manual:
    // this.router.navigateByUrl(`/courses/type/${encodeURIComponent(t.Type)}`);
  }
  }

